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

        public override Buttplug.Buttplug.Feature[] GetSupportedFeatures(Device device) =>
            device.DeviceMessages.RotateCmd;

        protected override IEnumerator HandleAnimation(DeviceFeature feature, StrokeInfo strokeInfo)
        {
            float completion = strokeInfo.Completion;
            float remaining = Mathf.Floor(completion * 2f) + 1f - completion;
            float strokeTimeSecs = strokeInfo.DurationSecs * remaining;
            float halfStrokeTimeSecs = strokeTimeSecs / 2f;
            float downSpeed = Mathf.Lerp(0.3f, 1f, 0.4f / strokeTimeSecs) *
                RotatorConfig.RotationSpeedRatio.Value;
            float upSpeed = downSpeed * 0.8f;
            Client.RotateCmd(feature, downSpeed, clockwise);
            yield return WaitForSecondsUnscaled(halfStrokeTimeSecs);
            Client.RotateCmd(feature, upSpeed, !clockwise);
            yield return WaitForSecondsUnscaled(halfStrokeTimeSecs);
            if (UnityEngine.Random.value <= RotatorConfig.RotationDirectionChangeChance.Value)
            {
                clockwise = !clockwise;
            }
        }

        protected override IEnumerator HandleOrgasm(DeviceFeature feature)
        {
            Client.RotateCmd(feature, 1f, clockwise);
            yield break;
        }

        protected override void HandleLevel(DeviceFeature feature, float level, float durationSecs) =>
            Client.RotateCmd(feature, level, true);
    }
}