using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public class RotatorController : StrokerController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone)
        {
            var random = new System.Random();
            bool clockwise = true;
            while (true)
            {
                if (game.IsIdle(girlIndex))
                {
                    client.RotateCmd(0, clockwise, girlIndex, bone);
                    yield return new WaitForSeconds(.1f);
                    continue;
                }
                float strokeTimeSecs = GetStrokeTimeSecs(girlIndex, bone);
                analyzer.TryGetWaveInfo(girlIndex, bone, out var waveInfo);
                for (int i = 0; i < waveInfo.Frequency - 1; i++)
                {
                    HandleCoroutine(DoRotate(girlIndex, bone, clockwise, strokeTimeSecs));
                    yield return new WaitForSecondsRealtime(strokeTimeSecs);
                }
                yield return HandleCoroutine(DoRotate(girlIndex, bone, clockwise, strokeTimeSecs));
                if (random.NextDouble() <= RotatorConfig.RotationDirectionChangeChance.Value)
                {
                    clockwise = !clockwise;
                }
            }
        }

        protected override void StopDevices(int girlIndex, Bone bone) =>
            client.RotateCmd(0f, true, girlIndex, bone);

        protected internal IEnumerator DoRotate(int girlIndex, Bone bone, bool clockwise,
            float strokeTimeSecs)
        {
            float downStrokeTimeSecs = strokeTimeSecs / 2f;
            float downSpeed = Mathf.Lerp(0.3f, 1f, 0.4f / strokeTimeSecs) *
                RotatorConfig.RotationSpeedRatio.Value;
            float upSpeed = downSpeed * 0.8f;
            client.RotateCmd(downSpeed, clockwise, girlIndex, bone);
            yield return new WaitForSecondsRealtime(downStrokeTimeSecs);
            client.RotateCmd(upSpeed, !clockwise, girlIndex, bone);
        }
    }
}
