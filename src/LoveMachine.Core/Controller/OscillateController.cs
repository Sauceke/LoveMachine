using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.Config;
using LoveMachine.Core.Game;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using LoveMachine.Core.Buttplug;
using UnityEngine;


namespace LoveMachine.Core.Controller
{
    internal sealed class OscillateController: ClassicButtplugController
    {
        public override bool IsDeviceSupported(Device device) => device.IsOscillate;

        protected override IEnumerator HandleAnimation(Device device, StrokeInfo strokeInfo) =>
          DoConstrict(device, GetPressure(device, strokeInfo));

        protected override IEnumerator HandleOrgasm(Device device) => DoConstrict(device, 1f);

        protected override void HandleLevel(Device device, float level, float durationSecs)
        { }

        private IEnumerator DoConstrict(Device device, float relativePressure)
        {
            var settings = device.Settings.OscillateSettings;
            float pressure = settings.Enabled
                ? Mathf.Lerp(settings.SpeedMin, settings.SpeedMax, t: relativePressure)
                : 0f;
            Client.OscillateCmd(device, pressure);
            yield return new WaitForSecondsRealtime(settings.UpdateIntervalSecs);
        }

        private float GetPressure(Device device, StrokeInfo strokeInfo)
        {
            switch (OscillateConfig.Mode.Value)
            {
                case OscillateConfig.OscillateMode.Cycle:
                    return GetSineBasedPressure();

                case OscillateConfig.OscillateMode.StrokeLength:
                    return GetStrokeLengthBasedPressure(strokeInfo);

                case OscillateConfig.OscillateMode.StrokeSpeed:
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
                1f / device.Settings.OscillateSettings.SpeedSensitivityMin,
                1f / device.Settings.OscillateSettings.SpeedSensitivityMax,
                value: strokeInfo.DurationSecs);

    }
}

