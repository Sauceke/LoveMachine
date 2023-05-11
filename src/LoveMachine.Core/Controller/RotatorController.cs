using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public sealed class RotatorController : ClassicButtplugController
    {
        private bool clockwise = true;

        protected override bool IsDeviceSupported(Device device) => device.IsRotator;

        protected override IEnumerator HandleAnimation(Device device, StrokeInfo strokeInfo)
        {
            float strokeTimeSecs = strokeInfo.DurationSecs;
            yield return HandleCoroutine(DoRotate(device, strokeTimeSecs));
            if (UnityEngine.Random.value <= RotatorConfig.RotationDirectionChangeChance.Value)
            {
                clockwise = !clockwise;
            }
        }

        protected override IEnumerator HandleOrgasm(Device device)
        {
            client.RotateCmd(device, 1f, clockwise);
            yield break;
        }

        private IEnumerator DoRotate(Device device, float strokeTimeSecs)
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