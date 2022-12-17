using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    internal class ConstrictController : ClassicButtplugController
    {
        protected override bool IsDeviceSupported(Device device) => device.IsConstrictor;

        protected override IEnumerator HandleAnimation(Device device)
        {
            float time = Time.time;
            float length = ConstrictConfig.CycleLengthSecs.Value;
            float pressure = Mathf.Lerp(
                device.Settings.ConstrictSettings.PressureMin,
                device.Settings.ConstrictSettings.PressureMax,
                t: Mathf.InverseLerp(-1f, 1f, value: Mathf.Sin(time * 2f * Mathf.PI / length)));
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
    }
}