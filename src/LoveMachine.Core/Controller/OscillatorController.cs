using System.Collections;
using System.Linq;
using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.Core.Controller
{
    internal class OscillatorController: ClassicButtplugController
    {
        public override string FeatureName => "Oscillation";

        public override bool IsDeviceSupported(Device device) => device.IsOscillator;

        protected override IEnumerator HandleAnimation(Device device, StrokeInfo strokeInfo)
        {
            var settings = device.Settings.OscillatorSettings;
            var feature = device.DeviceMessages.ScalarCmd
                .First(cmd => cmd.ActuatorType == Buttplug.Buttplug.Feature.Oscillate);
            int steps = feature.StepCount;
            float durationSecs = strokeInfo.DurationSecs;
            float rpm = 60f / durationSecs;
            float speed = Mathf.InverseLerp(0f, settings.MaxRpm, rpm);
            if (settings.SpeedMixing)
            {
                float lowerSpeed = Mathf.Floor(speed * steps) / steps;
                float upperSpeed = Mathf.Min(1f, lowerSpeed + 1f / steps);
                float lowerSecs = (upperSpeed - speed) * steps * durationSecs;
                float upperSecs = durationSecs - lowerSecs;
                if (lowerSecs > device.Settings.UpdatesHz)
                {
                    Client.OscillateCmd(device, lowerSpeed);
                }
                yield return WaitForSecondsUnscaled(lowerSecs);
                if (upperSecs > device.Settings.UpdatesHz)
                {
                    Client.OscillateCmd(device, upperSpeed);
                }
                yield return WaitForSecondsUnscaled(upperSecs);
                yield break;
            }
            Client.OscillateCmd(device, speed);
            yield return WaitForSecondsUnscaled(durationSecs);
        }

        protected override IEnumerator HandleOrgasm(Device device)
        {
            Client.OscillateCmd(device, 1f);
            yield break;
        }

        protected override void HandleLevel(Device device, float level, float durationSecs)
        {}
    }
}