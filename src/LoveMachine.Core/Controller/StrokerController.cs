using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    internal sealed class StrokerController : ClassicButtplugController
    {
        protected override bool IsDeviceSupported(Device device) => device.IsStroker;

        protected override IEnumerator HandleAnimation(Device device, StrokeInfo strokeInfo)
        {
            int updateFrequency = device.Settings.UpdatesHz;
            float durationSecs = strokeInfo.DurationSecs;
            // max number of subdivisions given the update frequency
            int subdivisions = 2 * (int)Mathf.Max(1f, durationSecs * updateFrequency / 2);
            int segments = device.Settings.StrokerSettings.SmoothStroking ? subdivisions : 2;
            float startCompletion = strokeInfo.Completion;
            yield return WaitWhile(() =>
                TryGetCurrentStrokeInfo(device, out var info) &&
                (int)(startCompletion * segments) == (int)(info.Completion * segments));
            if (!TryGetCurrentStrokeInfo(device, out strokeInfo))
            {
                yield break;
            }
            durationSecs = strokeInfo.DurationSecs;
            float refreshTimeSecs = durationSecs / segments;
            float refreshRate = 1f / segments;
            float completion = strokeInfo.Completion;
            float nextCompletion = completion + refreshRate;
            GetStrokeZone(durationSecs, device, strokeInfo, out float bottom, out float top);
            float currentPosition = Mathf.Lerp(bottom, top, Sinusoid(completion));
            float nextPosition = Mathf.Lerp(bottom, top, Sinusoid(nextCompletion));
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
            while (true)
            {
                client.LinearCmd(device, top, time);
                yield return new WaitForSecondsRealtime(time);
                client.LinearCmd(device, bottom, time);
                yield return new WaitForSecondsRealtime(time);
            }
        }

        protected override void HandleLevel(Device device, float level, float durationSecs) =>
            client.LinearCmd(device, level, durationSecs);

        private static float Sinusoid(float x) =>
            Mathf.InverseLerp(1f, -1f, Mathf.Cos(2 * Mathf.PI * x));

        private void GetStrokeZone(float strokeTimeSecs, Device device, StrokeInfo strokeInfo,
            out float min, out float max)
        {
            // decrease stroke length gradually as speed approaches the device limit
            float rate = 60f / device.Settings.StrokerSettings.MaxStrokesPerMin / strokeTimeSecs;
            float relativeLength = strokeInfo.Amplitude / game.PenisSize;
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