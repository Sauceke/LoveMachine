using System;
using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    internal class ConstrictController : ClassicButtplugController
    {
        protected override bool IsDeviceSupported(Device device) => device.IsConstrictor;

        protected override IEnumerator HandleAnimation(Device device) =>
            DoConstrict(device, GetPressure(device));

        protected override IEnumerator HandleOrgasm(Device device) => DoConstrict(device, 1f);

        private IEnumerator DoConstrict(Device device, float relativePressure)
        {
            var settings = device.Settings.ConstrictSettings;
            float pressure = settings.Enabled
                ? Mathf.Lerp(settings.PressureMin, settings.PressureMax, t: relativePressure)
                : 0f;
            client.ConstrictCmd(device, pressure);
            yield return new WaitForSecondsRealtime(settings.UpdateIntervalSecs);
        }

        private float GetPressure(Device device)
        {
            switch (ConstrictConfig.Mode.Value)
            {
                case ConstrictConfig.ConstrictMode.Cycle:
                    return GetSineBasedPressure();

                case ConstrictConfig.ConstrictMode.StrokeLength:
                    return GetStrokeLengthBasedPressure(device);

                case ConstrictConfig.ConstrictMode.StrokeSpeed:
                    return GetStrokeSpeedBasedPressure(device);
            }
            throw new Exception("unreachable");
        }

        private float GetSineBasedPressure() => Mathf.InverseLerp(-1f, 1f,
            value: Mathf.Sin(Time.time * 2f * Mathf.PI / ConstrictConfig.CycleLengthSecs.Value));

        private float GetStrokeLengthBasedPressure(Device device) =>
            analyzer.TryGetWaveInfo(device.Settings.GirlIndex, device.Settings.Bone, out var info)
                ? Mathf.InverseLerp(0, game.PenisSize, value: info.Amplitude)
                : 1f;

        private float GetStrokeSpeedBasedPressure(Device device)
        {
            var settings = device.Settings.ConstrictSettings;
            int girlIndex = device.Settings.GirlIndex;
            var bone = device.Settings.Bone;
            float freq = analyzer.TryGetWaveInfo(girlIndex, bone, out var info)
                ? info.Frequency
                : 1f;
            var strokeTimeSecs = GetAnimationTimeSecs(girlIndex) / freq;
            return Mathf.InverseLerp(
                settings.SpeedSensitivityMin,
                settings.SpeedSensitivityMax,
                value: strokeTimeSecs);
        }
    }
}