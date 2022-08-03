using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoveMachine.Core
{
    public class VibratorController : ButtplugController
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

        protected override bool IsDeviceSupported(Device device) => device.IsVibrator;

        protected override IEnumerator Run(Device device)
        {
            while (true)
            {
                if (game.IsIdle(device.Settings.GirlIndex))
                {
                    DoVibrate(device, 0f);
                    yield return new WaitForSeconds(.1f);
                    continue;
                }
                yield return HandleCoroutine(VibrateWithAnimation(device));
            }
        }

        protected IEnumerator VibrateWithAnimation(Device device)
        {
            int girlIndex = device.Settings.GirlIndex;
            var bone = device.Settings.Bone;
            float normalizedTime = GetLatencyCorrectedNormalizedTime(device);
            float phase = analyzer.TryGetWaveInfo(girlIndex, bone, out var result)
                ? result.Phase
                : 0f;
            float time = (normalizedTime - phase) % 1f;
            float frequency = result.Frequency;
            float strength = waveforms[device.Settings.VibratorSettings.Pattern](time * frequency);
            float intensity = Mathf.Lerp(
                device.Settings.VibratorSettings.IntensityMin,
                device.Settings.VibratorSettings.IntensityMax,
                t: strength * game.VibrationIntensity);
            DoVibrate(device, intensity);
            yield return new WaitForSecondsRealtime(1.0f / device.Settings.UpdatesHz);
        }

        private static float RectifiedSineWave(float x) => Mathf.Abs(Mathf.Cos(Mathf.PI * x));

        private static float TriangleWave(float x) => 2f * Mathf.Abs((x + 1f) % 1f - 0.5f);

        private static float SawWave(float x) => ((x % 1f) + 1f) % 1f;

        private static float PulseWave(float x) => Mathf.Round(SawWave(x));

        protected void DoVibrate(Device device, float intensity) =>
            client.VibrateCmd(device, intensity);
    }
}
