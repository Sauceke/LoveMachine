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

        // exact pose -> fractional part of normalized time at start of up-stroke
        protected static Dictionary<string, float> animPhases = new Dictionary<string, float>();

        // exact pose -> number of strokes per loop
        protected static Dictionary<string, int> animFreqs = new Dictionary<string, int>();

        private string GetExactPose(int girlIndex, int boneIndex)
            => GetPose(girlIndex) + "." + boneIndex;

        private float GetPhase(int girlIndex, int boneIndex)
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

        private int GetStrokesPerAnimationCycle(int girlIndex, int boneIndex)
        {
            animFreqs.TryGetValue(GetExactPose(girlIndex, boneIndex), out int strokes);
            return strokes == 0 ? 1 : strokes;
        }

        public void Awake()
        {
            client = gameObject.GetComponent<ButtplugWsClient>();
        }

        public void OnStartH() => StartCoroutine(RunLoops());

        public void OnEndH()
        {
            StopAllCoroutines();
            animPhases.Clear();
            for (int girlIndex = 0; girlIndex < HeroineCount; girlIndex++)
            {
                for (int boneIndex = 0; boneIndex < GetFemaleBones(girlIndex).Count + 1; boneIndex++)
                {
                    DoVibrate(0f, girlIndex, boneIndex);
                }
            }
        }

        private IEnumerator RunLoops()
        {
            yield return HandleCoroutine(UntilReady());
            for (int girlIndex = 0; girlIndex < HeroineCount; girlIndex++)
            {
                for (int boneIndex = 0; boneIndex < GetFemaleBones(girlIndex).Count + 1; boneIndex++)
                {
                    CoreConfig.Logger.LogDebug("Starting monitoring loop in controller " +
                        $"{GetType().Name} for girl index {girlIndex} and bone index " +
                        $"{boneIndex}.");
                    HandleCoroutine(Run(girlIndex, boneIndex));
                }
            }
        }

        protected IEnumerator RunStrokerLoop(int girlIndex, int boneIndex)
        {
            while (!IsHSceneInterrupted)
            {
                if (IsIdle(girlIndex))
                {
                    yield return new WaitForSeconds(.1f);
                    continue;
                }
                yield return HandleCoroutine(WaitForUpStroke(girlIndex, boneIndex));
                float strokeTimeSecs = GetStrokeTimeSecs(girlIndex, boneIndex);
                for (int i = 0; i < GetStrokesPerAnimationCycle(girlIndex, boneIndex) - 1; i++)
                {
                    HandleCoroutine(DoStroke(girlIndex, boneIndex, strokeTimeSecs));
                    yield return new WaitForSecondsRealtime(strokeTimeSecs);
                }
                yield return HandleCoroutine(DoStroke(girlIndex, boneIndex, strokeTimeSecs));
            }
        }

        protected IEnumerator RunVibratorLoop(int girlIndex, int boneIndex)
        {
            while (!IsHSceneInterrupted)
            {
                if (IsIdle(girlIndex))
                {
                    DoVibrate(0f, girlIndex, boneIndex);
                    yield return new WaitForSeconds(.1f);
                    continue;
                }
                yield return HandleCoroutine(VibrateWithAnimation(girlIndex, boneIndex,
                    VibrationIntensity));
            }
            // turn off vibration since there's nothing to animate against
            // this state can happen if H is ended while the animation is not in Idle
            DoVibrate(0.0f, girlIndex, boneIndex);
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

        private void OnDestroy() => StopAllCoroutines();

        protected void NerfAnimationSpeeds(float animStrokeTimeSecs, params Animator[] animators)
        {
            float speedMultiplier =
                Math.Min(1, animStrokeTimeSecs * CoreConfig.MaxStrokesPerMinute.Value / 60f);
            foreach (var animator in animators)
            {
                animator.speed = speedMultiplier;
            }
        }

        protected virtual float GetStrokeTimeSecs(int girlIndex, int boneIndex)
        {
            var info = GetAnimatorStateInfo(girlIndex);
            float strokeTimeSecs = info.length / info.speed
                / GetStrokesPerAnimationCycle(girlIndex, boneIndex);
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

        protected internal IEnumerator DoStroke(int girlIndex, int boneIndex,
            float strokeTimeSecs, bool forceHard = false)
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
            // needs to be real time so we can test devices even when the game is paused
            yield return new WaitForSecondsRealtime(strokeTimeSecs * 0.75f - downStrokeTimeSecs / 2f);
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

        protected void DoVibrate(float intensity, int girlIndex, int boneIndex)
        {
            client.VibrateCmd(intensity, girlIndex, boneIndex);
        }

        protected IEnumerator WaitForUpStroke(int girlIndex, int boneIndex)
        {
            AnimatorStateInfo info() => GetAnimatorStateInfo(girlIndex);
            float startNormTime = info().normalizedTime;
            float phase = GetPhase(girlIndex, boneIndex);
            float strokeTimeSecs = GetStrokeTimeSecs(girlIndex, boneIndex);
            float latencyNormTime = CoreConfig.LatencyMs.Value / 1000f / strokeTimeSecs;
            phase -= latencyNormTime;
            while ((int)(info().normalizedTime - phase + 2) == (int)(startNormTime - phase + 2))
            {
                yield return new WaitForSeconds(.01f);
            }
        }

        protected IEnumerator VibrateWithAnimation(int girlIndex, int boneIndex, float scale)
        {
            AnimatorStateInfo info() => GetAnimatorStateInfo(girlIndex);
            float strength = 1f;
            if (CoreConfig.SyncVibrationWithAnimation.Value)
            {
                // Simple cos based intensity amplification based on normalized position in looping animation
                float phase = GetPhase(girlIndex, boneIndex);
                float depth = (info().normalizedTime - phase) % 1;
                strength = Mathf.Abs(Mathf.Cos(Mathf.PI * depth)) + 0.1f;
            }
            float intensityPercent = Mathf.Lerp(CoreConfig.VibrationIntensityMin.Value,
                CoreConfig.VibrationIntensityMax.Value, strength * scale);
            float intensity = Mathf.InverseLerp(0f, 100f, intensityPercent);
            DoVibrate(intensity, girlIndex, boneIndex);
            yield return new WaitForSecondsRealtime(1.0f / CoreConfig.VibrationUpdateFrequency.Value);
        }

        private IEnumerator ComputeAnimationOffsets(int girlIndex)
        {
            string pose = GetExactPose(girlIndex, -1);
            var boneM = GetMaleBone();
            var femaleBones = GetFemaleBones(girlIndex);
            var measurements = new List<Measurement>();
            yield return new WaitForSeconds(0.1f);
            float currentTime = GetAnimatorStateInfo(girlIndex).normalizedTime;
            while (GetAnimatorStateInfo(girlIndex).normalizedTime - 1 < currentTime)
            {
                yield return new WaitForEndOfFrame();
                for (int i = 0; i < femaleBones.Count; i++)
                {
                    var boneF = femaleBones[i];
                    float distanceSq = (boneM.position - boneF.position).sqrMagnitude;
                    measurements.Add(new Measurement
                    {
                        BoneIndex = i,
                        Time = GetAnimatorStateInfo(girlIndex).normalizedTime,
                        DistanceSq = distanceSq
                    });
                }
            }
            if (GetExactPose(girlIndex, -1) != pose)
            {
                CoreConfig.Logger.LogWarning($"Pose {pose} interrupted; canceling calibration.");
                animPhases.Remove(pose);
                yield break;
            }
            for (int i = 0; i < femaleBones.Count; i++)
            {
                animPhases[GetExactPose(girlIndex, i + 1)] = measurements
                    .Where(entry => entry.BoneIndex == i)
                    .OrderBy(entry => entry.DistanceSq)
                    .FirstOrDefault()
                    .Time % 1;
                animFreqs[GetExactPose(girlIndex, i + 1)] = GetFrequency(measurements
                    .Where(entry => entry.BoneIndex == i)
                    .OrderBy(entry => entry.Time)
                    .Select(entry => entry.DistanceSq));
            }
            var closest = measurements
                .OrderBy(entry => entry.DistanceSq)
                .FirstOrDefault();
            animPhases[GetExactPose(girlIndex, 0)] = closest.Time % 1;
            animFreqs[GetExactPose(girlIndex, 0)] =
                animFreqs[GetExactPose(girlIndex, closest.BoneIndex + 1)];
            CoreConfig.Logger.LogDebug($"Calibration for pose {pose} completed. " +
                $"{measurements.Count / femaleBones.Count} frames inspected. " +
                $"Closest bone index: {closest.BoneIndex}, offset: {closest.Time % 1}, " +
                $"frequency: {animFreqs[GetExactPose(girlIndex, 0)]}. ");
        }

        private static int GetFrequency(IEnumerable<float> samples)
        {
            // catch flatlines
            if (samples.Max() - samples.Min() <= 0.000001f)
            {
                return 1;
            }
            // get frequency using Fourier series
            var dfsMagnitudes = new List<float>();
            // probably no game has more than 10 strokes in a loop
            for (int k = 1; k < 10; k++)
            {
                float freq = 2f * Mathf.PI / samples.Count() * k;
                float re = samples.Select((amp, index) => amp * Mathf.Cos(freq * index)).Sum();
                float im = samples.Select((amp, index) => amp * Mathf.Sin(freq * index)).Sum();
                dfsMagnitudes.Add(re * re + im * im);
            }
            return dfsMagnitudes.IndexOf(dfsMagnitudes.Max()) + 1;
        }

        protected AnimatorStateInfo GetAnimatorStateInfo(int girlIndex) =>
            GetFemaleAnimator(girlIndex).GetCurrentAnimatorStateInfo(AnimationLayer);

        protected abstract int HeroineCount { get; }
        protected abstract bool IsHardSex { get; }
        protected abstract int AnimationLayer { get; }
        protected abstract bool IsHSceneInterrupted { get; }

        protected virtual float VibrationIntensity { get; } = 1f;

        protected abstract Animator GetFemaleAnimator(int girlIndex);
        protected abstract List<Transform> GetFemaleBones(int girlIndex);
        protected abstract Transform GetMaleBone();
        protected abstract string GetPose(int girlIndex);
        protected abstract bool IsIdle(int girlIndex);
        protected abstract IEnumerator UntilReady();
        protected abstract IEnumerator Run(int girlIndex, int boneIndex);

        private struct Measurement
        {
            public int BoneIndex { get; set; }
            public float Time { get; set; }
            public float DistanceSq { get; set; }
        }
    }
}
