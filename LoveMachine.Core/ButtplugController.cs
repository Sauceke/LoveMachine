using System;
using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public abstract class ButtplugController : AnimationAnalyzer
    {
        private ButtplugWsClient client;

        protected abstract int HeroineCount { get; }
        protected abstract bool IsHardSex { get; }
        protected abstract bool IsHSceneInterrupted { get; }

        protected virtual float VibrationIntensity { get; } = 1f;

        protected abstract IEnumerator UntilReady();
        protected abstract IEnumerator Run(int girlIndex, Bone bone);

        public void Awake()
        {
            client = gameObject.GetComponent<ButtplugWsClient>();
        }

        public void OnStartH() => StartCoroutine(RunLoops());

        public void OnEndH()
        {
            StopAllCoroutines();
            for (int girlIndex = 0; girlIndex < HeroineCount; girlIndex++)
            {
                StopAnalyze(girlIndex);
                foreach (var bone in GetSupportedBones(girlIndex))
                {
                    DoVibrate(0f, girlIndex, bone);
                }
            }
            ClearCache();
        }

        private IEnumerator RunLoops()
        {
            yield return HandleCoroutine(UntilReady());
            for (int girlIndex = 0; girlIndex < HeroineCount; girlIndex++)
            {
                StartAnalyze(girlIndex);
                foreach (var bone in GetSupportedBones(girlIndex))
                {
                    CoreConfig.Logger.LogInfo("Starting monitoring loop in controller " +
                        $"{GetType().Name} for girl index {girlIndex} and bone {bone}. ");
                    HandleCoroutine(Run(girlIndex, bone));
                }
            }
            yield return new WaitUntil(() => IsHSceneInterrupted);
            OnEndH();
        }

        protected IEnumerator RunStrokerLoop(int girlIndex, Bone bone)
        {
            while (true)
            {
                if (IsIdle(girlIndex))
                {
                    yield return new WaitForSeconds(.1f);
                    continue;
                }
                yield return WaitForUpStroke(girlIndex, bone);
                float strokeTimeSecs = GetStrokeTimeSecs(girlIndex, bone);
                TryGetWaveInfo(girlIndex, bone, out var result);
                for (int i = 0; i < result.Frequency - 1; i++)
                {
                    HandleCoroutine(DoStroke(girlIndex, bone, strokeTimeSecs));
                    yield return new WaitForSecondsRealtime(strokeTimeSecs);
                }
                yield return HandleCoroutine(DoStroke(girlIndex, bone, strokeTimeSecs));
            }
        }

        protected IEnumerator RunVibratorLoop(int girlIndex, Bone bone)
        {
            while (true)
            {
                if (IsIdle(girlIndex))
                {
                    DoVibrate(0f, girlIndex, bone);
                    yield return new WaitForSeconds(.1f);
                    continue;
                }
                yield return HandleCoroutine(VibrateWithAnimation(girlIndex, bone,
                    VibrationIntensity));
            }
        }

        private void OnDestroy() => StopAllCoroutines();

        protected void NerfAnimationSpeeds(float animStrokeTimeSecs, params Animator[] animators)
        {
            float speedMultiplier =
                Math.Min(1, animStrokeTimeSecs * CoreConfig.MaxStrokesPerMinute.Value / 60f);
            foreach (var animator in animators)
            {
                animator.speed = Mathf.Min(animator.speed, speedMultiplier);
            }
        }

        protected internal IEnumerator DoStroke(int girlIndex, Bone bone,
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
            MoveStroker(
                position: max,
                durationSecs: strokeTimeSecs / 2f - 0.01f,
                girlIndex,
                bone);
            // needs to be real time so we can test devices even when the game is paused
            yield return new WaitForSecondsRealtime(strokeTimeSecs * 0.75f - downStrokeTimeSecs / 2f);
            MoveStroker(
                position: min,
                durationSecs: downStrokeTimeSecs - 0.01f,
                girlIndex,
                bone);
        }

        protected void MoveStroker(float position, float durationSecs, int girlIndex, Bone bone)
        {
            client.LinearCmd(position, durationSecs, girlIndex, bone);
        }

        protected void DoVibrate(float intensity, int girlIndex, Bone bone)
        {
            client.VibrateCmd(intensity, girlIndex, bone);
        }

        protected CustomYieldInstruction WaitForUpStroke(int girlIndex, Bone bone)
        {
            AnimatorStateInfo info() => GetAnimatorStateInfo(girlIndex);
            float startNormTime = info().normalizedTime;
            float strokeTimeSecs = GetStrokeTimeSecs(girlIndex, bone);
            float latencyNormTime = CoreConfig.LatencyMs.Value / 1000f / strokeTimeSecs;
            bool timeToStroke() => TryGetWaveInfo(girlIndex, bone, out var result)
                && (int)(info().normalizedTime - result.Phase + latencyNormTime + 10f)
                    != (int)(startNormTime - result.Phase + latencyNormTime + 10f);
            return new WaitUntil(timeToStroke);
        }

        protected IEnumerator VibrateWithAnimation(int girlIndex, Bone bone, float scale)
        {
            AnimatorStateInfo info() => GetAnimatorStateInfo(girlIndex);
            float strength = 1f;
            if (CoreConfig.SyncVibrationWithAnimation.Value)
            {
                // Simple cos based intensity amplification based on normalized position in looping animation
                float phase = TryGetWaveInfo(girlIndex, bone, out var result) ? result.Phase : 0f;
                float depth = (info().normalizedTime - phase) % 1;
                strength = Mathf.Abs(Mathf.Cos(Mathf.PI * depth)) + 0.1f;
            }
            float intensityPercent = Mathf.Lerp(CoreConfig.VibrationIntensityMin.Value,
                CoreConfig.VibrationIntensityMax.Value, strength * scale);
            float intensity = Mathf.InverseLerp(0f, 100f, intensityPercent);
            DoVibrate(intensity, girlIndex, bone);
            yield return new WaitForSecondsRealtime(1.0f / CoreConfig.VibrationUpdateFrequency.Value);
        }
    }
}
