using System;
using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    internal sealed class ConstrictController : ClassicButtplugController
    {
        protected override bool IsDeviceSupported(Device device) => device.IsConstrictor;

        protected override IEnumerator HandleAnimation(Device device, StrokeInfo strokeInfo) =>
            DoConstrict(device, GetPressure(device, strokeInfo));

        protected override IEnumerator HandleOrgasm(Device device) => DoConstrict(device, 1f);
        
        protected override void HandleLevel(Device device, float level, float durationSecs)
        { }

        private IEnumerator DoConstrict(Device device, float relativePressure)
        {
            var settings = device.Settings.ConstrictSettings;
            float pressure = settings.Enabled
                ? Mathf.Lerp(settings.PressureMin, settings.PressureMax, t: relativePressure)
                : 0f;
            client.ConstrictCmd(device, pressure);
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
            Mathf.InverseLerp(0, game.PenisSize, value: strokeInfo.Amplitude);

        private float GetStrokeSpeedBasedPressure(Device device, StrokeInfo strokeInfo) =>
            Mathf.InverseLerp(
                1f / device.Settings.ConstrictSettings.SpeedSensitivityMin,
                1f / device.Settings.ConstrictSettings.SpeedSensitivityMax,
                value: strokeInfo.DurationSecs);
    }
}