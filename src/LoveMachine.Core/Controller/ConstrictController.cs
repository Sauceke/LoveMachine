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

        protected override IEnumerator HandleAnimation(DeviceFeature feature, StrokeInfo strokeInfo) =>
            DoConstrict(feature, GetPressure(feature.Device, strokeInfo));

        protected override IEnumerator HandleOrgasm(DeviceFeature feature) => DoConstrict(feature, 1f);
        
        protected override void HandleLevel(DeviceFeature feature, float level, float durationSecs)
        { }

        private IEnumerator DoConstrict(DeviceFeature feature, float relativePressure)
        {
            var settings = feature.Device.Settings.ConstrictSettings;
            var pressureRange = settings.PressureRange;
            float pressure = settings.Enabled
                ? Mathf.Lerp(pressureRange.Min, pressureRange.Max, t: relativePressure)
                : 0f;
            Client.ConstrictCmd(feature, pressure);
            yield return new WaitForSecondsRealtime(settings.UpdateIntervalSecs);
        }

        private float GetPressure(Device device, StrokeInfo strokeInfo)
        {
            switch (ConstrictConfig.Mode.Value)
            {
                case ConstrictConfig.ConstrictMode.Cycle:
                    return GetSineBasedPressure();

                case ConstrictConfig.ConstrictMode.StrokeLength:
                    return GetStrokeLengthBasedPressure(strokeInfo);

                case ConstrictConfig.ConstrictMode.StrokeSpeed:
                    return GetStrokeSpeedBasedPressure(device, strokeInfo);
            }
            throw new Exception("unreachable");
        }

        private float GetSineBasedPressure() => Mathf.InverseLerp(-1f, 1f,
            value: Mathf.Sin(Time.time * 2f * Mathf.PI / ConstrictConfig.CycleLengthSecs.Value));

        private float GetStrokeLengthBasedPressure(StrokeInfo strokeInfo) =>
            Mathf.InverseLerp(0, Game.PenisSize, value: strokeInfo.Amplitude);

        private float GetStrokeSpeedBasedPressure(Device device, StrokeInfo strokeInfo) =>
            Mathf.InverseLerp(
                1f / device.Settings.ConstrictSettings.SpeedSensitivityRange.Min,
                1f / device.Settings.ConstrictSettings.SpeedSensitivityRange.Max,
                value: strokeInfo.DurationSecs);
    }
}