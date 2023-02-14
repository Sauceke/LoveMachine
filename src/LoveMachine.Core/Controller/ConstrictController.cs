using System;
using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    internal class ConstrictController : ClassicButtplugController
    {
        protected override bool IsDeviceSupported(Device device) => device.IsConstrictor;

        protected override IEnumerator HandleAnimation(Device device, WaveInfo waveInfo) =>
                DoConstrict(device, GetPressure(device, waveInfo));

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

        private float GetPressure(Device device, WaveInfo waveInfo)
        {
            switch (ConstrictConfig.Mode.Value)
            {
                case ConstrictConfig.ConstrictMode.Cycle:
                    return GetSineBasedPressure();

                case ConstrictConfig.ConstrictMode.StrokeLength:
                    return GetStrokeLengthBasedPressure(waveInfo);

                case ConstrictConfig.ConstrictMode.StrokeSpeed:
                    return GetStrokeSpeedBasedPressure(device, waveInfo);
            }
            throw new Exception("unreachable");
        }

        private float GetSineBasedPressure() => Mathf.InverseLerp(-1f, 1f,
            value: Mathf.Sin(Time.time * 2f * Mathf.PI / ConstrictConfig.CycleLengthSecs.Value));

        private float GetStrokeLengthBasedPressure(WaveInfo waveInfo) =>
                Mathf.InverseLerp(0, game.PenisSize, value: waveInfo.Amplitude);

        private float GetStrokeSpeedBasedPressure(Device device, WaveInfo waveInfo) =>
                Mathf.InverseLerp(
                    1f / device.Settings.ConstrictSettings.SpeedSensitivityMin,
                    1f / device.Settings.ConstrictSettings.SpeedSensitivityMax,
                    value: GetAnimationTimeSecs(device) / waveInfo.Frequency);
    }
}