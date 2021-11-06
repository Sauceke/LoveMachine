using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoveMachine.Core
{
    public abstract class ButtplugController : MonoBehaviour
    {
        private ButtplugWsClient client;

        // animation -> fractional part of normalized time at start of up-stroke
        protected static Dictionary<string, float> animPhases = new Dictionary<string, float>();

        private string GetExactPose(int girlIndex, int boneIndex)
        {
            return GetPose(girlIndex) + "." + boneIndex;
        }

        protected float GetPhase(int girlIndex, int boneIndex)
        {
            string pose = GetExactPose(girlIndex, -1);
            if (!animPhases.ContainsKey(pose))
            {
                animPhases[pose] = 0; // avoid multiple interleaving calls
                HandleCoroutine(ComputeAnimationOffsets(girlIndex));
            }
            animPhases.TryGetValue(GetExactPose(girlIndex, boneIndex), out float phase);
            return phase;
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
            for (int girlIndex = 0; girlIndex < HeroineCount; girlIndex++)
            {
                for (int boneIndex = 0; boneIndex < GetFemaleBones(girlIndex).Count + 1; boneIndex++)
                {
                    HandleCoroutine(Run(girlIndex, boneIndex));
                }
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

        protected internal IEnumerator DoStroke(float strokeTimeSecs, int girlIndex, int boneIndex, bool forceHard = false)
        {
            float minSlow = Mathf.InverseLerp(0, 100, CoreConfig.SlowStrokeZoneMin.Value);
            float maxSlow = Mathf.InverseLerp(0, 100, CoreConfig.SlowStrokeZoneMax.Value);
            float minFast = Mathf.InverseLerp(0, 100, CoreConfig.FastStrokeZoneMin.Value);
            float maxFast = Mathf.InverseLerp(0, 100, CoreConfig.FastStrokeZoneMax.Value);
            float hardness = forceHard || IsHardSex
                ? Mathf.InverseLerp(0, 100, CoreConfig.HardSexIntensity.Value)
                : 0;
            // decrease stroke length gradually as speed approaches the device limit
            float rate = 60f / CoreConfig.MaxStrokesPerMinute.Value / strokeTimeSecs;
            float min = Mathf.Lerp(minSlow, minFast, rate);
            float max = Mathf.Lerp(maxSlow, maxFast, rate);
            float downStrokeTimeSecs = Mathf.Lerp(strokeTimeSecs / 2f, strokeTimeSecs / 4f, hardness);
            client.LinearCmd(
                position: max,
                durationSecs: strokeTimeSecs / 2f - 0.01f,
                girlIndex,
                boneIndex);
            yield return new WaitForSeconds(strokeTimeSecs * 0.75f - downStrokeTimeSecs / 2f);
            client.LinearCmd(
                position: min,
                durationSecs: downStrokeTimeSecs - 0.01f,
                girlIndex,
                boneIndex);
        }

        protected void MoveStroker(float position, float durationSecs, int girlIndex, int boneIndex)
        {
            client.LinearCmd(position, durationSecs, girlIndex, boneIndex);
        }

        protected void DoVibrate(float intensity, int girlIndex, int boneIndex = 0)
        {
            client.VibrateCmd(intensity, girlIndex, boneIndex);
        }

        protected IEnumerator WaitForUpStroke(Func<AnimatorStateInfo> info, int girlIndex, int boneIndex)
        {
            var initialState = info();
            float startNormTime = initialState.normalizedTime;
            float phase = GetPhase(girlIndex, boneIndex);
            float strokeTimeSecs = GetStrokeTimeSecs(initialState);
            float latencyNormTime = CoreConfig.LatencyMs.Value / 1000f / strokeTimeSecs;
            phase -= latencyNormTime;
            while ((int)(info().normalizedTime - phase + 2) == (int)(startNormTime - phase + 2))
            {
                yield return new WaitForSeconds(.01f);
            }
        }

        protected IEnumerator VibrateWithAnimation(AnimatorStateInfo info, int girlIndex,
            int boneIndex, float intensity, float minVibration)
        {
            float strength = 1f;
            if (CoreConfig.SyncVibrationWithAnimation.Value)
            {
                // Simple cos based intensity amplification based on normalized position in looping animation
                float phase = GetPhase(girlIndex, boneIndex);
                float depth = (info.normalizedTime - phase) % 1;
                strength = Mathf.Abs(Mathf.Cos(Mathf.PI * depth)) + 0.1f;
            }
            DoVibrate(Mathf.Lerp(minVibration, 1.0f, strength * intensity), girlIndex);
            yield return new WaitForSecondsRealtime(1.0f / CoreConfig.VibrationUpdateFrequency.Value);
        }

        private IEnumerator ComputeAnimationOffsets(int girlIndex)
        {
            var femaleAnimator = GetFemaleAnimator(girlIndex);
            var maleAnimator = GetMaleAnimator();
            var boneM = GetMaleBone();
            var femaleBones = GetFemaleBones(girlIndex);
            var measurements = new List<Measurement>();
            for (float normTime = 0; normTime < 1; normTime += .1f)
            {
                femaleAnimator.Play(CurrentAnimationStateHash, AnimationLayer, normTime);
                maleAnimator.Play(CurrentAnimationStateHash, AnimationLayer, normTime);
                yield return new WaitForEndOfFrame();
                for (int i = 0; i < femaleBones.Count; i++)
                {
                    var boneF = femaleBones[i];
                    float distanceSq = (boneM.position - boneF.position).sqrMagnitude;
                    measurements.Add(new Measurement
                    {
                        BoneIndex = i,
                        Time = normTime,
                        DistanceSq = distanceSq
                    });
                }
            }
            for (int i = 0; i < femaleBones.Count; i++)
            {
                animPhases[GetExactPose(girlIndex, i + 1)] = measurements
                    .Where(entry => entry.BoneIndex == i)
                    .OrderBy(entry => entry.DistanceSq)
                    .FirstOrDefault()
                    .Time;
            }
            animPhases[GetExactPose(girlIndex, 0)] = measurements
                    .OrderBy(entry => entry.DistanceSq)
                    .FirstOrDefault()
                    .Time;
            // rewind so that non-looping animations don't end abruptly
            femaleAnimator.Play(CurrentAnimationStateHash, AnimationLayer, 0);
            maleAnimator.Play(CurrentAnimationStateHash, AnimationLayer, 0);
        }

        protected abstract int HeroineCount { get; }
        protected abstract bool IsHardSex { get; }
        protected abstract int AnimationLayer { get; }
        protected abstract int CurrentAnimationStateHash { get; }
        protected abstract Animator GetFemaleAnimator(int girlIndex);
        protected abstract Animator GetMaleAnimator();
        protected abstract List<Transform> GetFemaleBones(int girlIndex);
        protected abstract Transform GetMaleBone();
        protected abstract string GetPose(int girlIndex);
        protected abstract IEnumerator UntilReady();
        protected abstract IEnumerator Run(int girlIndex, int boneIndex);

        public enum VibrationMode
        {
            Off = 0_00,
            Male = 0_01,
            Female = 0_10,
            Both = 0_11
        }

        private struct Measurement
        {
            public int BoneIndex { get; set; }
            public float Time { get; set; }
            public float DistanceSq { get; set; }
        }
    }
}
