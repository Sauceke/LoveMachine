using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ButtPlugin.Core
{
    public abstract class ButtplugController : MonoBehaviour
    {
        private ButtplugWsClient client;
        
        // animation -> fractional part of normalized time at start of up-stroke
        protected Dictionary<string, float> animPhases = new Dictionary<string, float>();

        protected IEnumerator CalculatePhase(int girlIndex, Action<float> callback)
        {
            string pose = GetPose(girlIndex);
            if (!animPhases.ContainsKey(pose))
            {
                yield return HandleCoroutine(ComputeAnimationOffset(girlIndex, pose));
            }
            callback(animPhases[pose]);
        }

        public bool IsFemale
        {
            get { return (CoreConfig.EnableVibrate.Value & VibrationMode.Female) == VibrationMode.Female; }
        }

        public bool IsMale
        {
            get { return (CoreConfig.EnableVibrate.Value & VibrationMode.Male) == VibrationMode.Male; }
        }

        public void Awake()
        {
            client = gameObject.GetComponent<ButtplugWsClient>();
        }

        public void OnStartH()
        {
            StartCoroutine(RunLoops());
        }

        private IEnumerator RunLoops()
        {
            yield return HandleCoroutine(UntilReady());
            for (int i = 0; i < HeroineCount; i++)
            {
                HandleCoroutine(Run(girlIndex: i));
            }
        }

        protected Coroutine HandleCoroutine(IEnumerator coroutine)
        {
            return StartCoroutine(HandleExceptions(coroutine));
        }

        protected IEnumerator HandleExceptions(IEnumerator coroutine)
        {
            while (TryNext(coroutine))
            {
                yield return coroutine.Current;
            }
        }

        private bool TryNext(IEnumerator coroutine)
        {
            try
            {
                return coroutine.MoveNext();
            }
            catch (Exception e)
            {
                CoreConfig.Logger.LogError($"Coroutine failed with exception: {e}");
                throw e;
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        protected void NerfAnimationSpeeds(float animStrokeTimeSecs, params Animator[] animators)
        {
            float speedMultiplier =
                Math.Min(1, animStrokeTimeSecs * CoreConfig.MaxStrokesPerMinute.Value / 60f);
            foreach (var animator in animators)
            {
                animator.speed = speedMultiplier;
            }
        }

        protected float GetStrokeTimeSecs(AnimatorStateInfo info)
        {
            float strokeTimeSecs = info.length / info.speed;
            // sometimes the length of an animation becomes Infinity in KK
            // sometimes the speed becomes 0 in HS2
            // this is a catch-all for god knows what other things that can
            // possibly go wrong and cause the stroking coroutine to hang
            if (strokeTimeSecs > 10 || strokeTimeSecs < 0.001f
                || float.IsNaN(strokeTimeSecs))
            {
                return .01f;
            }
            return strokeTimeSecs;
        }

        protected IEnumerator DoStroke(float strokeTimeSecs, int girlIndex)
        {
            int strokeTimeMs = (int)(strokeTimeSecs * 1000) - 10;
            // decrease stroke length gradually as speed approaches the device limit
            double rate = 60f / CoreConfig.MaxStrokesPerMinute.Value / strokeTimeSecs;
            double margin = rate * rate * 0.3;
            client.LinearCmd(
                position: 1 - margin * 0.7,
                durationMs: strokeTimeMs / 2,
                girlIndex);
            yield return new WaitForSeconds(strokeTimeSecs / 2f);
            client.LinearCmd(
                position: margin * 0.3,
                durationMs: strokeTimeMs / 2,
                girlIndex);
        }

        protected void DoVibrate(float intensity, int girlIndex)
        {
            client.VibrateCmd(intensity, girlIndex);
        }

        protected IEnumerator WaitForUpStroke(Func<AnimatorStateInfo> info, int girlIndex)
        {
            var initialState = info();
            float startNormTime = initialState.normalizedTime;
            float phase = 0;
            yield return HandleCoroutine(CalculatePhase(girlIndex, result => phase = result));
            float strokeTimeSecs = GetStrokeTimeSecs(initialState);
            float latencyNormTime = CoreConfig.LatencyMs.Value / 1000f / strokeTimeSecs;
            phase -= latencyNormTime;
            while ((int)(info().normalizedTime - phase + 2) == (int)(startNormTime - phase + 2))
            {
                yield return new WaitForSeconds(.01f);
            }
        }

        protected IEnumerator VibrateWithAnimation(AnimatorStateInfo info, int girlIndex,
            float intensity, float minVibration)
        {
            float strength = 1f;
            if (CoreConfig.SyncVibrationWithAnimation.Value)
            {
                // Simple cos based intensity amplification based on normalized position in looping animation
                float phase = 0;
                yield return HandleCoroutine(CalculatePhase(girlIndex, result => phase = result));
                float depth = (info.normalizedTime - phase) % 1;
                strength = Mathf.Abs(Mathf.Cos(Mathf.PI * depth)) + 0.1f;
            }
            DoVibrate(Mathf.Lerp(minVibration, 1.0f, strength * intensity), girlIndex);
            yield return new WaitForSecondsRealtime(1.0f / CoreConfig.VibrationUpdateFrequency.Value);
        }

        private IEnumerator ComputeAnimationOffset(int girlIndex, string pose)
        {
            float minDistanceSq = float.MaxValue;
            float minDistanceNormTime = 0;
            var femaleAnimator = GetFemaleAnimator(girlIndex);
            var maleAnimator = GetMaleAnimator();
            for (float normTime = 0; normTime < 1; normTime += .1f)
            {
                femaleAnimator.Play(CurrentAnimationStateHash, AnimationLayer, normTime);
                maleAnimator.Play(CurrentAnimationStateHash, AnimationLayer, normTime);
                yield return new WaitForEndOfFrame();
                foreach (var bone1 in GetMaleBones())
                {
                    foreach (var bone2 in GetFemaleBones(girlIndex))
                    {
                        float distanceSq = (bone1.position - bone2.position).sqrMagnitude;
                        if (distanceSq < minDistanceSq)
                        {
                            minDistanceSq = distanceSq;
                            minDistanceNormTime = normTime;
                        }
                    }
                }
            }
            animPhases[pose] = minDistanceNormTime;
        }

        protected abstract int HeroineCount { get; }
        protected abstract int AnimationLayer { get; }
        protected abstract int CurrentAnimationStateHash { get; }
        protected abstract Animator GetFemaleAnimator(int girlIndex);
        protected abstract Animator GetMaleAnimator();
        protected abstract List<Transform> GetFemaleBones(int girlIndex);
        protected abstract List<Transform> GetMaleBones();
        protected abstract string GetPose(int girlIndex);
        protected abstract IEnumerator UntilReady();
        protected abstract IEnumerator Run(int girlIndex);

        public enum VibrationMode
        {
            Off = 0_00,
            Male = 0_01,
            Female = 0_10,
            Both = 0_11
        }
    }
}
