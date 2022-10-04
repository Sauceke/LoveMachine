using System;
using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public sealed class StrokerController : ClassicButtplugController
    {
        public StrokerController(IntPtr handle) : base(handle) { }

        protected override bool IsDeviceSupported(Device device) => device.IsStroker;

        protected override IEnumerator HandleAnimation(Device device)
        {
            int girlIndex = device.Settings.GirlIndex;
            var bone = device.Settings.Bone;
            if (!analyzer.TryGetWaveInfo(girlIndex, bone, out var waveInfo))
            {
                yield break;
            }
            int updateFrequency = device.Settings.UpdatesHz;
            float animTimeSecs = GetAnimationTimeSecs(girlIndex);
            // min number of subdivisions
            int turns = 2 * waveInfo.Frequency;
            // max number of subdivisions given the update frequency
            int subdivisions = turns * (int)Mathf.Max(1f, animTimeSecs * updateFrequency / turns);
            int segments = device.Settings.StrokerSettings.SmoothStroking ? subdivisions : turns;
            float startNormTime = GetLatencyCorrectedNormalizedTime(device);
            int getSegment(float time) => (int)((time - waveInfo.Phase) * segments);
            yield return new WaitWhile((Func<bool>)(() =>
                getSegment(GetLatencyCorrectedNormalizedTime(device)) == getSegment(startNormTime)));
            animTimeSecs = GetAnimationTimeSecs(girlIndex);
            float refreshTimeSecs = animTimeSecs / segments;
            float refreshNormTime = 1f / segments;
            float currentNormTime = GetLatencyCorrectedNormalizedTime(device);
            float nextNormTime = currentNormTime + refreshNormTime;
            GetStrokeZone(animTimeSecs, device, waveInfo, out float bottom, out float top);
            float currentPosition = Mathf.Lerp(bottom, top, Sinusoid(currentNormTime, waveInfo));
            float nextPosition = Mathf.Lerp(bottom, top, Sinusoid(nextNormTime, waveInfo));
            bool movingUp = currentPosition < nextPosition;
            float targetPosition = movingUp ? top : bottom;
            float speed = (nextPosition - currentPosition) / refreshTimeSecs;
            speed *= movingUp ? 1f : 1f + game.StrokingIntensity;
            float timeToTargetSecs = (targetPosition - currentPosition) / speed;
            client.LinearCmd(device, targetPosition, timeToTargetSecs);
        }

        protected override IEnumerator HandleOrgasm(Device device)
        {
            float bottom = StrokerConfig.OrgasmDepth.Value;
            float time = 0.5f / StrokerConfig.OrgasmShakingFrequency.Value;
            float top = bottom + device.Settings.StrokerSettings.MaxStrokesPerMin / 60f / 2f * time;
            float startTime = Time.realtimeSinceStartup;
            while (game.IsOrgasming(device.Settings.GirlIndex)
                || Time.realtimeSinceStartup - startTime < game.MinOrgasmDurationSecs)
            {
                client.LinearCmd(device, top, time);
                yield return new WaitForSecondsRealtime(time);
                client.LinearCmd(device, bottom, time);
                yield return new WaitForSecondsRealtime(time);
            }
        }

        private static float Sinusoid(float x, AnimationAnalyzer.WaveInfo waveInfo) =>
            Mathf.InverseLerp(1f, -1f,
                Mathf.Cos(2 * Mathf.PI * waveInfo.Frequency * (x - waveInfo.Phase)));

        private void GetStrokeZone(float strokeTimeSecs, Device device,
            AnimationAnalyzer.WaveInfo waveInfo, out float min, out float max)
        {
            // decrease stroke length gradually as speed approaches the device limit
            float rate = 60f / device.Settings.StrokerSettings.MaxStrokesPerMin / strokeTimeSecs;
            float relativeLength = waveInfo.Amplitude / game.PenisSize;
            float scale = Mathf.Lerp(
                1f - StrokerConfig.StrokeLengthRealism.Value,
                1f,
                t: relativeLength);
            min = scale * Mathf.Lerp(
                device.Settings.StrokerSettings.SlowStrokeZoneMin,
                device.Settings.StrokerSettings.FastStrokeZoneMin,
                t: rate);
            max = scale * Mathf.Lerp(
                device.Settings.StrokerSettings.SlowStrokeZoneMax,
                device.Settings.StrokerSettings.FastStrokeZoneMax,
                t: rate);
        }
    }
}
