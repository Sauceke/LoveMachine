using System;
using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    /// <summary>
    /// If you want to add a new device type, you'll probably need to extend
    /// this class.
    /// </summary>
    internal abstract class ClassicButtplugController : ButtplugController
    {
        /// <summary>
        /// How to react to the current state of the animation. <br/>
        /// This will be called repeatedly while there is a sex animation
        /// playing AND the character assigned to this device is not currently
        /// climaxing. <br/>
        /// This is also used for testing, so do NOT do anything here that
        /// depends on some external state that isn't available outside of
        /// H-scenes.
        /// </summary>
        protected abstract IEnumerator HandleAnimation(Device device, StrokeInfo strokeInfo);

        /// <summary>
        /// How to react to orgasms. <br/>
        /// This will be stopped automatically once the orgasm is over.
        /// </summary>
        protected abstract IEnumerator HandleOrgasm(Device device);

        /// <summary>
        /// If this device type has a (preferably safe) way to handle a given
        /// float value between 0.0 and 1.0 (level) over a set duration
        /// (durationSecs), then you can do that here.
        /// If it doesn't, then do nothing.
        /// </summary>
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
                if (base.TryGetCurrentStrokeInfo(device, out var strokeInfo))
                {
                    yield return HandleCoroutine(HandleAnimation(device, strokeInfo));
                }
                else
                {
                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }
        }

        [Obsolete("Do NOT use this; use the StrokeInfo provided as parameter.")]
        protected override bool TryGetCurrentStrokeInfo(Device device, out StrokeInfo result) =>
            throw new NotImplementedException();

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
            yield return HandleCoroutine(EmulateStrokes(device, 2, 2f, display));
            yield return HandleCoroutine(EmulateStrokes(device, 2, 1f, display));
            yield return HandleCoroutine(EmulateStrokes(device, 5, 0.3f, display));
        }

        private IEnumerator HandleStroke(Device device, float durationSecs) =>
            EmulateStrokes(device, count: 1, durationSecs, position => { });

        private IEnumerator EmulateStrokes(Device device, int count, float durationSecs,
            DisplayPosition display)
        {
            float startTime = Time.unscaledTime;
            while (Time.unscaledTime < startTime + count * durationSecs)
            {
                float completion = ((Time.unscaledTime - startTime) / durationSecs) % 1f;
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