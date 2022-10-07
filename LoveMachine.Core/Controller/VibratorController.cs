using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoveMachine.Core
{
    public sealed class VibratorController : ClassicButtplugController
    {
        private static readonly Dictionary<VibrationPattern, Func<float, float>> waveforms =
            new Dictionary<VibrationPattern, Func<float, float>>
            {
                { VibrationPattern.Sine, RectifiedSineWave },
                { VibrationPattern.Triangle, TriangleWave },
                { VibrationPattern.Saw, SawWave },
                { VibrationPattern.Pulse, PulseWave },
                { VibrationPattern.Constant, _ => 1f }
            };

        public VibratorController(IntPtr handle) : base(handle) { }

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
            float strength = CurrentWave(time * frequency, device.Settings.VibratorSettings);
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

        private static float CurrentWave(float x, VibratorSettings settings) =>
            settings.Pattern == VibrationPattern.Custom
                ? CustomWave(x, settings.CustomPattern)
                : waveforms[settings.Pattern](x);

        private static float RectifiedSineWave(float x) => Mathf.Abs(Mathf.Cos(Mathf.PI * x));

        private static float TriangleWave(float x) => 2f * Mathf.Abs((x + 1f) % 1f - 0.5f);

        private static float SawWave(float x) => (x % 1f + 1f) % 1f;

        private static float PulseWave(float x) => Mathf.Round(SawWave(x));

        private static float CustomWave(float x, float[] pattern) =>
            pattern[(int)((x % 1f + 1f) % 1f * pattern.Length)];
    }
}
