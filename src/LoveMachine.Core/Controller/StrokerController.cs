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
            // on a sine curve, 4 linear segments is mathematically the same as 2
            subdivisions = (settings.Pattern == StrokingPattern.Sine && subdivisions == 4)
                ? 2
                : subdivisions;
            int segments = settings.SmoothStroking ? subdivisions : 2;
            float startCompletion = strokeInfo.Completion;
            float nextSegmentCompletion = Mathf.Round(startCompletion * segments + 1) / segments;
            float timeToNextSegmentSecs = (nextSegmentCompletion - startCompletion) * durationSecs;
            GetStrokeZone(feature.Device.Settings, strokeInfo, out float bottom, out float top);
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
            var zone = feature.Device.Settings.StrokerSettings.OrgasmShakeZone;
            float secs = 1f / feature.Device.Settings.UpdatesHz;
            while (true)
            {
                Client.LinearCmd(feature, zone.Max, secs);
                yield return new WaitForSecondsRealtime(secs);
                Client.LinearCmd(feature, zone.Min, secs);
                yield return new WaitForSecondsRealtime(secs);
            }
        }

        protected override void HandleLevel(DeviceFeature feature, float level, float durationSecs) =>
            Client.LinearCmd(feature, level, durationSecs);

        public float GetPosition(float x, StrokerSettings settings)
        {
            if (!settings.SmoothStroking)
            {
                return SineWave(x);
            }
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

        private void GetStrokeZone(DeviceSettings settings, StrokeInfo strokeInfo,
            out float min, out float max)
        {
            min = settings.StrokerSettings.StrokeZone.Min;
            max = settings.StrokerSettings.StrokeZone.Max;
            // scale down according to intensity
            float scale = GetIntensity(StrokerConfig.IntensitySettings, settings, strokeInfo);
            max = Mathf.Lerp(min, max, scale);
        }
    }
}