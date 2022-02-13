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

        protected abstract bool IsIdle(int girlIndex);
        protected abstract IEnumerator UntilReady();
        protected abstract IEnumerator Run(int girlIndex, int boneIndex);

        public void Awake()
        {
            client = gameObject.GetComponent<ButtplugWsClient>();
        }

        public void OnStartH() => StartCoroutine(RunLoops());

        public void OnEndH()
        {
            StopAllCoroutines();
            ClearMeasurements();
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
                    CoreConfig.Logger.LogInfo("Starting monitoring loop in controller " +
                        $"{GetType().Name} for girl index {girlIndex} and bone index " +
                        $"{boneIndex}.");
                    HandleCoroutine(Run(girlIndex, boneIndex));
                }
            }
            yield return new WaitUntil(() => IsHSceneInterrupted);
            OnEndH();
        }

        protected IEnumerator RunStrokerLoop(int girlIndex, int boneIndex)
        {
            while (true)
            {
                if (IsIdle(girlIndex))
                {
                    yield return new WaitForSeconds(.1f);
                    continue;
                }
                yield return WaitForUpStroke(girlIndex, boneIndex);
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
            while (true)
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

        protected CustomYieldInstruction WaitForUpStroke(int girlIndex, int boneIndex)
        {
            AnimatorStateInfo info() => GetAnimatorStateInfo(girlIndex);
            float startNormTime = info().normalizedTime;
            float strokeTimeSecs = GetStrokeTimeSecs(girlIndex, boneIndex);
            float latencyNormTime = CoreConfig.LatencyMs.Value / 1000f / strokeTimeSecs;
            bool timeToStroke() => TryGetPhase(girlIndex, boneIndex, out float phase)
                && (int)(info().normalizedTime - phase + latencyNormTime + 10f)
                    != (int)(startNormTime - phase + latencyNormTime + 10f);
            return new WaitUntil(timeToStroke);
        }

        protected IEnumerator VibrateWithAnimation(int girlIndex, int boneIndex, float scale)
        {
            AnimatorStateInfo info() => GetAnimatorStateInfo(girlIndex);
            float strength = 1f;
            if (CoreConfig.SyncVibrationWithAnimation.Value)
            {
                // Simple cos based intensity amplification based on normalized position in looping animation
                if (!TryGetPhase(girlIndex, boneIndex, out float phase))
                {
                    phase = 0;
                }
                float depth = (info().normalizedTime - phase) % 1;
                strength = Mathf.Abs(Mathf.Cos(Mathf.PI * depth)) + 0.1f;
            }
            float intensityPercent = Mathf.Lerp(CoreConfig.VibrationIntensityMin.Value,
                CoreConfig.VibrationIntensityMax.Value, strength * scale);
            float intensity = Mathf.InverseLerp(0f, 100f, intensityPercent);
            DoVibrate(intensity, girlIndex, boneIndex);
            yield return new WaitForSecondsRealtime(1.0f / CoreConfig.VibrationUpdateFrequency.Value);
        }
    }
}
