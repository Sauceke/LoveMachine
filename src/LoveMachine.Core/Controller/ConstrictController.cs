using System;
using System.Collections;
using System.Linq;
using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Config;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.Core.Controller
{
    internal sealed class ConstrictController : ClassicButtplugController
    {
        public override string FeatureName => "Pressure";

        public override Buttplug.Buttplug.Feature[] GetSupportedFeatures(Device device) =>
            device.DeviceMessages.ScalarCmd.Where(feature => feature.IsConstrictor).ToArray();

        protected override IEnumerator HandleAnimation(DeviceFeature feature,
            StrokeInfo strokeInfo) =>
            DoConstrict(feature, GetIntensity(
                ConstrictConfig.IntensitySettings, feature.Device.Settings, strokeInfo));

        protected override IEnumerator HandleOrgasm(DeviceFeature feature) => DoConstrict(feature, 1f);
        
        protected override void HandleLevel(DeviceFeature feature, float level, float durationSecs)
        { }

        private IEnumerator DoConstrict(DeviceFeature feature, float relativePressure)
        {
            var settings = feature.Device.Settings.ConstrictSettings;
            var pressureRange = settings.PressureRange;
            float pressure = Mathf.Lerp(pressureRange.Min, pressureRange.Max, t: relativePressure);
            Client.ConstrictCmd(feature, pressure);
            yield return new WaitForSecondsRealtime(settings.UpdateIntervalSecs);
        }
    }
}