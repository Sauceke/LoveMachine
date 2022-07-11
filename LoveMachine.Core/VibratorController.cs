using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public class VibratorController : ButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone)
        {
            while (true)
            {
                if (game.IsIdle(girlIndex))
                {
                    DoVibrate(0f, girlIndex, bone);
                    yield return new WaitForSeconds(.1f);
                    continue;
                }
                yield return HandleCoroutine(VibrateWithAnimation(girlIndex, bone));
            }
        }

        protected override void StopDevices(int girlIndex, Bone bone) =>
            DoVibrate(0f, girlIndex, bone);

        protected IEnumerator VibrateWithAnimation(int girlIndex, Bone bone)
        {
            game.GetAnimState(girlIndex, out float normalizedTime, out _, out _);
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
            float intensityPercent = Mathf.Lerp(VibratorConfig.VibrationIntensityMin.Value,
                VibratorConfig.VibrationIntensityMax.Value, strength * game.VibrationIntensity);
            float intensity = Mathf.InverseLerp(0f, 100f, intensityPercent);
            DoVibrate(intensity, girlIndex, bone);
            yield return new WaitForSecondsRealtime(
                1.0f / VibratorConfig.VibrationUpdateFrequency.Value);
        }

        protected void DoVibrate(float intensity, int girlIndex, Bone bone) =>
            client.VibrateCmd(intensity, girlIndex, bone);
    }
}
