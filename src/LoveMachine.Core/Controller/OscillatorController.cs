using System;
using System.Collections;
using System.Linq;
using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Config;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.Core.Controller
{
    internal sealed class OscillatorController: ClassicButtplugController
    {
        public override string FeatureName => "Oscillation";

        public override bool IsDeviceSupported(Device device) => device.IsOscillator;

        protected override IEnumerator HandleAnimation(Device device, StrokeInfo strokeInfo)
        {
            switch (OscillatorConfig.Mode.Value)
            {
                case OscillatorConfig.OscillationMode.Speed:
                    return OscillateOnSpeed(device, strokeInfo);
                
                case OscillatorConfig.OscillationMode.Depth:
                    return OscillateOnDepth(device, strokeInfo);
            }
            throw new Exception("unreachable");
        }

        protected override IEnumerator HandleOrgasm(Device device)
        {
            Client.OscillateCmd(device, 1f);
            yield break;
        }

        protected override void HandleLevel(Device device, float level, float durationSecs)
        {}

        private IEnumerator OscillateOnSpeed(Device device, StrokeInfo strokeInfo)
        {
            var settings = device.Settings.OscillatorSettings;
            var feature = device.DeviceMessages.ScalarCmd
                .First(cmd => cmd.ActuatorType == Buttplug.Buttplug.Feature.Oscillate);
            int steps = feature.StepCount;
            float durationSecs = strokeInfo.DurationSecs;
            float rpm = Mathf.Min(60f / durationSecs, OscillatorConfig.RpmLimit.Value);
            float level = Mathf.InverseLerp(settings.MinRpm, settings.MaxRpm, rpm);
            float speed = Mathf.Lerp(1f / steps, 1f, level);
            if (!settings.SpeedMixing)
            {
                Client.OscillateCmd(device, speed);
                yield return WaitForSecondsUnscaled(durationSecs);
                yield break;
            }
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
        }
        
        private IEnumerator OscillateOnDepth(Device device, StrokeInfo strokeInfo)
        {
            var settings = device.Settings.OscillatorSettings;
            if (OscillatorConfig.Mode.Value == OscillatorConfig.OscillationMode.Depth)
            {
                float depth = Mathf.PingPong(strokeInfo.Completion * 2f + 1f, 1f);
                float rpm = Mathf.Lerp(0f, OscillatorConfig.RpmLimit.Value, depth);
                float speed = Mathf.InverseLerp(0f, settings.MaxRpm, rpm);
                Client.OscillateCmd(device, speed);
                yield return WaitForSecondsUnscaled(1f / device.Settings.UpdatesHz);
            }
        }
    }
}