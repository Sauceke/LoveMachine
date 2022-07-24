using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public class VibratorController : ButtplugController
    {
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
            float strength = 1f;
            if (VibratorConfig.SyncVibrationWithAnimation.Value)
            {
                // Simple cos based intensity amplification based on normalized position in
                // looping animation
                float phase = analyzer.TryGetWaveInfo(girlIndex, bone, out var result)
                    ? result.Phase
                    : 0f;
                float time = (normalizedTime - phase) % 1;
                float frequency = result.Frequency;
                strength = Mathf.Abs(Mathf.Cos(Mathf.PI * time * frequency)) + 0.1f;
            }
            float intensity = Mathf.Lerp(
                device.Settings.VibratorSettings.IntensityMin,
                device.Settings.VibratorSettings.IntensityMax,
                t: strength * game.VibrationIntensity);
            DoVibrate(device, intensity);
            yield return new WaitForSecondsRealtime(1.0f / device.Settings.UpdatesHz);
        }

        protected void DoVibrate(Device device, float intensity) =>
            client.VibrateCmd(device, intensity);
    }
}
