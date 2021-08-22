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
        protected static Dictionary<string, float> animPhases = new Dictionary<string, float>();

        protected float GetPhase(int girlIndex)
        {
            string pose = GetPose(girlIndex);
            if (!animPhases.ContainsKey(pose))
            {
                animPhases[pose] = 0; // avoid multiple interleaving calls
                HandleCoroutine(ComputeAnimationOffset(girlIndex, pose));
            }
            return animPhases[pose];
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

        protected internal Coroutine HandleCoroutine(IEnumerator coroutine)
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

        protected internal IEnumerator DoStroke(float strokeTimeSecs, int girlIndex)
        {
            int strokeTimeMs = (int)(strokeTimeSecs * 1000) - 10;
            float minSlow = Mathf.InverseLerp(0, 100, CoreConfig.SlowStrokeZoneMin.Value);
            float maxSlow = Mathf.InverseLerp(0, 100, CoreConfig.SlowStrokeZoneMax.Value);
            float minFast = Mathf.InverseLerp(0, 100, CoreConfig.FastStrokeZoneMin.Value);
            float maxFast = Mathf.InverseLerp(0, 100, CoreConfig.FastStrokeZoneMax.Value);
            // decrease stroke length gradually as speed approaches the device limit
            float rate = 60f / CoreConfig.MaxStrokesPerMinute.Value / strokeTimeSecs;
            float min = Mathf.Lerp(minSlow, minFast, rate);
            float max = Mathf.Lerp(maxSlow, maxFast, rate);
            client.LinearCmd(
                position: max,
                durationMs: strokeTimeMs / 2,
                girlIndex);
            yield return new WaitForSeconds(strokeTimeSecs / 2f);
            client.LinearCmd(
                position: min,
                durationMs: strokeTimeMs / 2,
                girlIndex);
        }

        protected void MoveStroker(float position, float durationSecs, int girlIndex, int actionIndex)
        {
            int durationMs = (int)(durationSecs * 1000);
            client.LinearCmd(position, durationMs, girlIndex, actionIndex);
        }

        protected void DoVibrate(float intensity, int girlIndex, int actionIndex = 0)
        {
            client.VibrateCmd(intensity, girlIndex, actionIndex);
        }

        protected IEnumerator WaitForUpStroke(Func<AnimatorStateInfo> info, int girlIndex)
        {
            var initialState = info();
            float startNormTime = initialState.normalizedTime;
            float phase = GetPhase(girlIndex);
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
                float phase = GetPhase(girlIndex);
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
