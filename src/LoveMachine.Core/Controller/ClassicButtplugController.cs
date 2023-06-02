using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoveMachine.Core
{
    internal abstract class ClassicButtplugController : ButtplugController
    {
        protected abstract IEnumerator HandleAnimation(Device device, StrokeInfo strokeInfo);

        protected abstract IEnumerator HandleOrgasm(Device device);

        protected abstract void HandleLevel(Device device, float level, float durationSecs);
        
        protected override IEnumerator Run(Device device)
        {
            foreach (var gimmick in GetComponents<Gimmick>())
            {
                Logger.LogInfo($"Running gimmick {gimmick.GetType()} for device " +
                    $"#{device.DeviceIndex} in controller {GetType()}.");
                HandleCoroutine(gimmick.Run(device, HandleLevel, HandleStroke));
            }
            while (true)
            {
                if (game.IsOrgasming(device.Settings.GirlIndex))
                {
                    var orgasm = HandleCoroutine(HandleOrgasm(device));
                    yield return new WaitForSecondsRealtime(game.MinOrgasmDurationSecs);
                    yield return WaitWhile(() => game.IsOrgasming(device.Settings.GirlIndex));
                    // unity may have destroyed the coroutine if it's already finished
                    if (orgasm != null)
                    {
                        StopCoroutine(orgasm);
                    }
                    continue;
                }
                if (IsIdleOrPaused(device))
                {
                    client.StopDeviceCmd(device);
                    while (IsIdleOrPaused(device))
                    {
                        yield return new WaitForSecondsRealtime(0.1f);
                    }
                    continue;
                }
                if (TryGetCurrentStrokeInfo(device, out var strokeInfo))
                {
                    yield return HandleCoroutine(HandleAnimation(device, strokeInfo));
                }
                else
                {
                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }
        }
        
        private bool IsIdleOrPaused(Device device) =>
            game.IsIdle(device.Settings.GirlIndex) || game.TimeScale == 0f;

        public void Test(Device device, DisplayPosition display) =>
            HandleCoroutine(RunTest(device, display));

        private IEnumerator RunTest(Device device, DisplayPosition display)
        {
            if (!IsDeviceSupported(device))
            {
                yield break;
            }
            float[] durations = Enumerable.Repeat(2f, 2)
                .Concat(Enumerable.Repeat(1f, 2))
                .Concat(Enumerable.Repeat(0.3f, 5))
                .ToArray();
            foreach (float duration in durations)
            {
                yield return HandleCoroutine(EmulateStroke(device, duration, display));
            }
        }

        private IEnumerator HandleStroke(Device device, float durationSecs) =>
            EmulateStroke(device, durationSecs, position => { });

        private IEnumerator EmulateStroke(Device device, float durationSecs,
            DisplayPosition display)
        {
            float startTime = Time.unscaledTime;
            while (Time.unscaledTime - startTime < durationSecs)
            {
                float completion = (Time.unscaledTime - startTime) / durationSecs;
                var strokeInfo = new StrokeInfo
                {
                    Amplitude = 1f,
                    Completion = completion,
                    DurationSecs = durationSecs
                };
                float position = (completion < 0.5f ? completion : 1f - completion) * 2f;
                display(position);
                yield return HandleCoroutine(HandleAnimation(device, strokeInfo));
            }
        }

        public delegate void DisplayPosition(float position);
    }
}