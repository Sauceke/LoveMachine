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
        public override bool IsDeviceSupported(Device device) => device.IsStroker;

        protected override IEnumerator HandleAnimation(Device device, StrokeInfo strokeInfo)
        {
            var settings = device.Settings.StrokerSettings;
            int updateFrequency = device.Settings.UpdatesHz;
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
            float currentPosition = Mathf.Lerp(bottom, top, Sinusoid(startCompletion));
            float nextPosition = Mathf.Lerp(bottom, top, Sinusoid(nextSegmentCompletion));
            bool movingUp = currentPosition < nextPosition;
            float targetPosition = movingUp ? top : bottom;
            float speed = (nextPosition - currentPosition) / timeToNextSegmentSecs;
            speed *= movingUp ? 1f : 1f + Game.StrokingIntensity;
            float timeToTargetSecs = (targetPosition - currentPosition) / speed;
            Client.LinearCmd(device, targetPosition, timeToTargetSecs);
            yield return WaitForSecondsUnscaled(timeToNextSegmentSecs - Time.deltaTime);
        }

        protected override IEnumerator HandleOrgasm(Device device)
        {
            float bottom = StrokerConfig.OrgasmDepth.Value;
            float time = 0.5f / StrokerConfig.OrgasmShakingFrequency.Value;
            float top = bottom + device.Settings.StrokerSettings.MaxStrokesPerMin / 60f / 2f * time;
            while (true)
            {
                Client.LinearCmd(device, top, time);
                yield return new WaitForSecondsRealtime(time);
                Client.LinearCmd(device, bottom, time);
                yield return new WaitForSecondsRealtime(time);
            }
        }

        protected override void HandleLevel(Device device, float level, float durationSecs) =>
            Client.LinearCmd(device, level, durationSecs);

        private static float Sinusoid(float x) =>
            Mathf.InverseLerp(1f, -1f, Mathf.Cos(2 * Mathf.PI * x));

        private void GetStrokeZone(float strokeTimeSecs, StrokerSettings settings,
            StrokeInfo strokeInfo, out float min, out float max)
        {
            // decrease stroke length gradually as speed approaches the device limit
            float rate = 60f / settings.MaxStrokesPerMin / strokeTimeSecs;
            float relativeLength = strokeInfo.Amplitude / Game.PenisSize;
            min = Mathf.Lerp(settings.SlowStrokeZoneMin, settings.FastStrokeZoneMin, t: rate);
            max = Mathf.Lerp(settings.SlowStrokeZoneMax, settings.FastStrokeZoneMax, t: rate);
            // scale down according to stroke length realism
            float realism = StrokerConfig.StrokeLengthRealism.Value;
            float scale = Mathf.Lerp(1f - realism, 1f, t: relativeLength);
            max = Mathf.Lerp(min, max, scale);
        }
    }
}