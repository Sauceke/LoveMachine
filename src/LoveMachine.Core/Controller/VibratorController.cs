using System;
using System.Collections;
using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.Core.Controller
{
    internal sealed class VibratorController : ClassicButtplugController
    {
        public override string FeatureName => "Vibration";
        
        public override bool IsDeviceSupported(Device device) => device.IsVibrator;

        protected override IEnumerator HandleAnimation(Device device, StrokeInfo strokeInfo)
        {
            float strength = GetStrength(strokeInfo.Completion, device.Settings.VibratorSettings);
            float intensity = Mathf.Lerp(
                device.Settings.VibratorSettings.IntensityMin,
                device.Settings.VibratorSettings.IntensityMax,
                t: strength * Game.VibrationIntensity);
            Client.VibrateCmd(device, intensity);
            yield return WaitForSecondsUnscaled(1f / device.Settings.UpdatesHz);
        }

        protected override IEnumerator HandleOrgasm(Device device)
        {
            Client.VibrateCmd(device, device.Settings.VibratorSettings.IntensityMax);
            yield break;
        }

        protected override void HandleLevel(Device device, float level, float durationSecs) =>
            Client.VibrateCmd(device, level);

        private static float GetStrength(float x, VibratorSettings settings)
        {
            switch (settings.Pattern)
            {
                case VibrationPattern.Sine:
                    return RectifiedSineWave(x);

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

        private static float RectifiedSineWave(float x) => Mathf.Abs(Mathf.Cos(Mathf.PI * x));

        private static float TriangleWave(float x) => 2f * Mathf.Abs((x + 1f) % 1f - 0.5f);

        private static float SawWave(float x) => (x % 1f + 1f) % 1f;

        private static float PulseWave(float x) => Mathf.Round(SawWave(x));

        private static float CustomWave(float x, float[] pattern) =>
            pattern[(int)((x % 1f + 1f) % 1f * pattern.Length)];
    }
}