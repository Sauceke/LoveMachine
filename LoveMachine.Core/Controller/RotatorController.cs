using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public class RotatorController : StrokerController
    {
        protected override bool IsDeviceSupported(Device device) => device.IsRotator;

        protected override IEnumerator Run(Device device)
        {
            var random = new System.Random();
            bool clockwise = true;
            while (true)
            {
                int girlIndex = device.Settings.GirlIndex;
                Bone bone = device.Settings.Bone;
                if (game.IsIdle(girlIndex))
                {
                    client.RotateCmd(device, 0, clockwise);
                    yield return new WaitForSeconds(.1f);
                    continue;
                }
                analyzer.TryGetWaveInfo(girlIndex, bone, out var waveInfo);
                float strokeTimeSecs = GetAnimationTimeSecs(girlIndex, bone) / waveInfo.Frequency;
                for (int i = 0; i < waveInfo.Frequency - 1; i++)
                {
                    HandleCoroutine(DoRotate(device, clockwise, strokeTimeSecs));
                    yield return new WaitForSecondsRealtime(strokeTimeSecs);
                }
                yield return HandleCoroutine(DoRotate(device, clockwise, strokeTimeSecs));
                if (random.NextDouble() <= RotatorConfig.RotationDirectionChangeChance.Value)
                {
                    clockwise = !clockwise;
                }
            }
        }

        protected internal IEnumerator DoRotate(Device device, bool clockwise,
            float strokeTimeSecs)
        {
            float downStrokeTimeSecs = strokeTimeSecs / 2f;
            float downSpeed = Mathf.Lerp(0.3f, 1f, 0.4f / strokeTimeSecs) *
                RotatorConfig.RotationSpeedRatio.Value;
            float upSpeed = downSpeed * 0.8f;
            client.RotateCmd(device, downSpeed, clockwise);
            yield return new WaitForSecondsRealtime(downStrokeTimeSecs);
            client.RotateCmd(device, upSpeed, !clockwise);
        }
    }
}
