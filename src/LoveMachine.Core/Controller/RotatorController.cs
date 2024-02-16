using System.Collections;
using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Config;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.Core.Controller
{
    internal sealed class RotatorController : ClassicButtplugController
    {
        public override string FeatureName => "Rotation";
        
        private bool clockwise = true;

        public override bool IsDeviceSupported(Device device) => device.IsRotator;

        protected override IEnumerator HandleAnimation(Device device, StrokeInfo strokeInfo)
        {
            float completion = strokeInfo.Completion;
            float remaining = Mathf.Floor(completion * 2f) + 1f - completion;
            float strokeTimeSecs = strokeInfo.DurationSecs * remaining;
            float halfStrokeTimeSecs = strokeTimeSecs / 2f;
            float downSpeed = Mathf.Lerp(0.3f, 1f, 0.4f / strokeTimeSecs) *
                RotatorConfig.RotationSpeedRatio.Value;
            float upSpeed = downSpeed * 0.8f;
            Client.RotateCmd(device, downSpeed, clockwise);
            yield return WaitForSecondsUnscaled(halfStrokeTimeSecs);
            Client.RotateCmd(device, upSpeed, !clockwise);
            yield return WaitForSecondsUnscaled(halfStrokeTimeSecs);
            if (UnityEngine.Random.value <= RotatorConfig.RotationDirectionChangeChance.Value)
            {
                clockwise = !clockwise;
            }
        }

        protected override IEnumerator HandleOrgasm(Device device)
        {
            Client.RotateCmd(device, 1f, clockwise);
            yield break;
        }

        protected override void HandleLevel(Device device, float level, float durationSecs) =>
            Client.RotateCmd(device, level, true);
    }
}