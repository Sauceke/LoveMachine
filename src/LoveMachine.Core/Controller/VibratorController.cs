using System;
using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public sealed class VibratorController : ClassicButtplugController
    {
        protected override bool IsDeviceSupported(Device device) => device.IsVibrator;

        protected override IEnumerator HandleAnimation(Device device)
        {
            int girlIndex = device.Settings.GirlIndex;
            var bone = device.Settings.Bone;
            float normalizedTime = GetLatencyCorrectedNormalizedTime(device);
            analyzer.TryGetWaveInfo(girlIndex, bone, out var result);
            float phase = result.Phase;
            float frequency = result.Frequency;
            float time = normalizedTime - phase;
            float strength = GetStrength(time * frequency, device.Settings.VibratorSettings);
            float intensity = Mathf.Lerp(
                device.Settings.VibratorSettings.IntensityMin,
                device.Settings.VibratorSettings.IntensityMax,
                t: strength * game.VibrationIntensity);
            client.VibrateCmd(device, intensity);
            yield return new WaitForSecondsRealtime(1.0f / device.Settings.UpdatesHz);
        }

        protected override IEnumerator HandleOrgasm(Device device)
        {
            client.VibrateCmd(device, device.Settings.VibratorSettings.IntensityMax);
            yield return new WaitForSecondsRealtime(game.MinOrgasmDurationSecs);
            yield return WaitWhile(() => game.IsOrgasming(device.Settings.GirlIndex));
            client.StopDeviceCmd(device);
        }

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