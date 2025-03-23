using System;
using System.Collections;
using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.Config;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.Core.Controller
{
    internal sealed class StrokerController : ClassicButtplugController
    {
        public override string FeatureName => "Position";
        
        public override Buttplug.Buttplug.Feature[] GetSupportedFeatures(Device device) =>
            device.DeviceMessages.LinearCmd;

        protected override IEnumerator HandleAnimation(DeviceFeature feature, StrokeInfo strokeInfo)
        {
            var settings = feature.Device.Settings.StrokerSettings;
            int updateFrequency = feature.Device.Settings.UpdatesHz;
            float durationSecs = strokeInfo.DurationSecs;
            // max number of subdivisions given the update frequency
            int subdivisions = 2 * (int)Mathf.Max(1f, durationSecs * updateFrequency / 2);
            // 4 subdivisions is mathematically the same as 2
            subdivisions = subdivisions == 4 ? 2 : subdivisions;
            int segments = settings.SmoothStroking ? subdivisions : 2;
            float startCompletion = strokeInfo.Completion;
            float nextSegmentCompletion = Mathf.Round(startCompletion * segments + 1) / segments;
            float timeToNextSegmentSecs = (nextSegmentCompletion - startCompletion) * durationSecs;
            GetStrokeZone(durationSecs, settings, strokeInfo, out float bottom, out float top);
            float currentPosition = Mathf.Lerp(bottom, top, GetPosition(startCompletion, settings));
            float nextPosition =
                Mathf.Lerp(bottom, top, GetPosition(nextSegmentCompletion, settings));
            bool movingUp = currentPosition < nextPosition;
            float targetPosition = movingUp ? top : bottom;
            float speed = (nextPosition - currentPosition) / timeToNextSegmentSecs;
            speed *= movingUp ? 1f : 1f + Game.StrokingIntensity;
            float timeToTargetSecs = (targetPosition - currentPosition) / speed;
            Client.LinearCmd(feature, targetPosition, timeToTargetSecs);
            yield return WaitForSecondsUnscaled(timeToNextSegmentSecs - Time.deltaTime);
        }

        protected override IEnumerator HandleOrgasm(DeviceFeature feature)
        {
            float bottom = StrokerConfig.OrgasmDepth.Value;
            float time = 0.5f / StrokerConfig.OrgasmShakingFrequency.Value;
            float top = bottom + feature.Device.Settings.StrokerSettings.MaxStrokesPerMin / 60f / 2f * time;
            while (true)
            {
                Client.LinearCmd(feature, top, time);
                yield return new WaitForSecondsRealtime(time);
                Client.LinearCmd(feature, bottom, time);
                yield return new WaitForSecondsRealtime(time);
            }
        }

        protected override void HandleLevel(DeviceFeature feature, float level, float durationSecs) =>
            Client.LinearCmd(feature, level, durationSecs);

        public float GetPosition(float x, StrokerSettings settings)
        {
            switch (settings.Pattern)
            {
                case StrokingPattern.Sine:
                    return SineWave(x);

                case StrokingPattern.Cups:
                    return CupsWave(x);

                case StrokingPattern.Arches:
                    return ArchesWave(x);

                case StrokingPattern.Custom:
                    return CustomWave(x, settings.CustomPattern);
            }
            throw new Exception("unreachable");
        }

        private static float SineWave(float x) =>
            Mathf.InverseLerp(1f, -1f, Mathf.Cos(2 * Mathf.PI * x));

        private static float CupsWave(float x) => 1 - Mathf.Abs(Mathf.Cos(Mathf.PI * x));

        private static float ArchesWave(float x) => Mathf.Abs(Mathf.Sin(Mathf.PI * x));

        private static float CustomWave(float x, float[] pattern) =>
            pattern[(int)(Mathf.Repeat(x, 1f) * pattern.Length)];

        private void GetStrokeZone(float strokeTimeSecs, StrokerSettings settings,
            StrokeInfo strokeInfo, out float min, out float max)
        {
            // decrease stroke length gradually as speed approaches the device limit
            float rate = 60f / settings.MaxStrokesPerMin / strokeTimeSecs;
            float relativeLength = strokeInfo.Amplitude / Game.PenisSize;
            min = Mathf.Lerp(settings.SlowStrokeZone.Min, settings.FastStrokeZone.Min, t: rate);
            max = Mathf.Lerp(settings.SlowStrokeZone.Max, settings.FastStrokeZone.Max, t: rate);
            // scale down according to stroke length realism
            float realism = StrokerConfig.StrokeLengthRealism.Value;
            float scale = Mathf.Lerp(1f - realism, 1f, t: relativeLength);
            max = Mathf.Lerp(min, max, scale);
        }
    }
}