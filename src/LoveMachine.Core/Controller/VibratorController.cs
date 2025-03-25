using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.Config;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.Core.Controller
{
    internal sealed class VibratorController : ClassicButtplugController
    {
        public override string FeatureName => "Vibration";
        
        public override Buttplug.Buttplug.Feature[] GetSupportedFeatures(Device device) =>
            device.DeviceMessages.ScalarCmd.Where(feature => feature.IsVibrator).ToArray();

        protected override IEnumerator HandleAnimation(DeviceFeature feature, StrokeInfo strokeInfo)
        {
            float strength =
                GetStrength(strokeInfo.Completion, feature.Device.Settings.VibratorSettings);
            strength *=
                GetIntensity(VibratorConfig.IntensitySettings, feature.Device.Settings, strokeInfo);
            float intensity = Mathf.Lerp(
                feature.Device.Settings.VibratorSettings.IntensityRange.Min,
                feature.Device.Settings.VibratorSettings.IntensityRange.Max,
                t: strength);
            Client.VibrateCmd(feature, intensity);
            yield return WaitForSecondsUnscaled(1f / feature.Device.Settings.UpdatesHz);
        }

        protected override IEnumerator HandleOrgasm(DeviceFeature feature)
        {
            Client.VibrateCmd(feature, feature.Device.Settings.VibratorSettings.IntensityRange.Max);
            yield break;
        }

        protected override void HandleLevel(DeviceFeature feature, float level, float durationSecs) =>
            Client.VibrateCmd(feature, level);

        private static float GetStrength(float x, VibratorSettings settings)
        {
            switch (settings.Pattern)
            {
                case VibrationPattern.Sine:
                    return AbsSineWave(x);

                case VibrationPattern.Triangle:
                    return TriangleWave(x);

                case VibrationPattern.Saw:
                    return SawWave(x);

                case VibrationPattern.Pulse:
                    return PulseWave(x);

                case VibrationPattern.Constant:
                    return 1f;

                case VibrationPattern.Custom:
                    return CustomWave(x, settings.CustomPattern);
            }
            throw new Exception("unreachable");
        }

        private static float AbsSineWave(float x) => Mathf.Abs(Mathf.Cos(Mathf.PI * x));

        private static float TriangleWave(float x) => Mathf.PingPong(x * 2f + 1f, 1f);

        private static float SawWave(float x) => Mathf.Repeat(x, 1f);

        private static float PulseWave(float x) => Mathf.Round(SawWave(x));

        private static float CustomWave(float x, float[] pattern) =>
            pattern[(int)(SawWave(x) * pattern.Length)];
    }
}