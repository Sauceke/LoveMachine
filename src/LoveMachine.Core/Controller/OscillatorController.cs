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

        public override Buttplug.Buttplug.Feature[] GetSupportedFeatures(Device device) =>
            device.DeviceMessages.ScalarCmd.Where(feature => feature.IsOscillator).ToArray();

        protected override IEnumerator HandleAnimation(DeviceFeature feature, StrokeInfo strokeInfo)
        {
            OscillateWithRpm(feature, 60f / strokeInfo.DurationSecs);
            yield return WaitForSecondsUnscaled(strokeInfo.DurationSecs);
        }

        protected override IEnumerator HandleOrgasm(DeviceFeature feature)
        {
            OscillateWithRpm(feature, OscillatorConfig.RpmLimit.Value);
            yield break;
        }

        protected override void HandleLevel(DeviceFeature feature, float level, float durationSecs)
        {}

        private void OscillateWithRpm(DeviceFeature feature, float rpm)
        {
            rpm = Mathf.Min(rpm, OscillatorConfig.RpmLimit.Value);
            var settings = feature.Device.Settings.OscillatorSettings;
            int steps = feature.Feature.StepCount;
            float rate = Mathf.InverseLerp(settings.RpmRange.Min, settings.RpmRange.Max, value: rpm);
            float speed = Mathf.Lerp(1f / steps, 1f, t: rate);
            Client.OscillateCmd(feature, speed);
        }
    }
}