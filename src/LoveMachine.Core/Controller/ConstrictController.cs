using System;
using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    internal class ConstrictController : ClassicButtplugController
    {
        protected override bool IsDeviceSupported(Device device) => device.IsConstrictor;

        protected override IEnumerator HandleAnimation(Device device)
        {
            float pressure = Mathf.Lerp(
                device.Settings.ConstrictSettings.PressureMin,
                device.Settings.ConstrictSettings.PressureMax,
                t: GetPressure(device));
            client.ConstrictCmd(device, pressure);
            yield return new WaitForSecondsRealtime(
                device.Settings.ConstrictSettings.UpdateIntervalSecs);
        }

        protected override IEnumerator HandleOrgasm(Device device)
        {
            client.ConstrictCmd(device, device.Settings.ConstrictSettings.PressureMax);
            yield return new WaitForSecondsRealtime(
                device.Settings.ConstrictSettings.UpdateIntervalSecs);
        }

        private float GetPressure(Device device)
        {
            switch (ConstrictConfig.Mode.Value)
            {
                case ConstrictConfig.ConstrictMode.Cycle:
                    return GetSineBasedPressure();

                case ConstrictConfig.ConstrictMode.StrokeLength:
                    return GetStrokeLengthBasedPressure(device);
            }
            throw new Exception("unreachable");
        }

        private float GetSineBasedPressure() => Mathf.InverseLerp(-1f, 1f,
            value: Mathf.Sin(Time.time * 2f * Mathf.PI / ConstrictConfig.CycleLengthSecs.Value));

        private float GetStrokeLengthBasedPressure(Device device) =>
            analyzer.TryGetWaveInfo(device.Settings.GirlIndex, device.Settings.Bone, out var info)
                ? Mathf.InverseLerp(0, game.PenisSize, value: info.Amplitude)
                : 1f;
    }
}