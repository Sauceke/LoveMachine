using System;
using System.Collections;
using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Controller.Addons;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.Core.Controller
{
    /// <summary>
    /// More refined version of ButtplugController with stroke emulation
    /// capabilities.
    /// If you want to add a new device type, you'll probably need to extend
    /// this class.
    /// </summary>
    internal abstract class ClassicButtplugController : ButtplugController
    {
        /// <summary>
        /// How to react to the current state of the animation. <br/>
        /// This will be called repeatedly while there is a sex animation
        /// playing AND the character assigned to this device is not currently
        /// climaxing.
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
            Coroutine strokeLoop = null;
            const float refreshTimeSecs = 0.3f;
            while (true)
            {
                yield return WaitForSecondsUnscaled(refreshTimeSecs);
                if (Game.IsOrgasming(device.Settings.GirlIndex))
                {
                    TryStopCoroutine(ref strokeLoop);
                    var orgasm = HandleCoroutine(HandleOrgasm(device));
                    yield return WaitForSecondsUnscaled(Game.MinOrgasmDurationSecs);
                    while (Game.IsOrgasming(device.Settings.GirlIndex))
                    {
                        yield return WaitForSecondsUnscaled(refreshTimeSecs);
                    }
                    TryStopCoroutine(ref orgasm);
                    continue;
                }
                if (IsIdleOrPaused(device))
                {
                    TryStopCoroutine(ref strokeLoop);
                    Client.StopDeviceCmd(device);
                    while (IsIdleOrPaused(device))
                    {
                        yield return WaitForSecondsUnscaled(refreshTimeSecs);
                    }
                    continue;
                }
                strokeLoop = strokeLoop ?? HandleCoroutine(RunStrokeLoop(device));
            }
        }

        private IEnumerator RunStrokeLoop(Device device)
        {
            while (true)
            {
                if (!base.TryGetCurrentStrokeInfo(device, out var strokeInfo))
                {
                    yield return WaitForSecondsUnscaled(0.1f);
                    continue;
                }
                // unwrap the coroutine so we can interrupt it midway
                var handleAnimation = HandleAnimation(device, strokeInfo);
                while (handleAnimation.MoveNext())
                {
                    yield return handleAnimation.Current;
                }
            }
        }

        protected override bool TryGetCurrentStrokeInfo(Device device, out StrokeInfo result) =>
            throw new NotImplementedException("Do NOT use this; use the strokeInfo parameter!");

        private bool IsIdleOrPaused(Device device) =>
            Game.IsIdle(device.Settings.GirlIndex) || Time.timeScale == 0f;

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
            Client.StopDeviceCmd(device);
        }

        private IEnumerator HandleStroke(Device device, float durationSecs)
        {
            yield return HandleCoroutine(EmulateStrokes(device, count: 1, durationSecs, _ => { }));
            Client.StopDeviceCmd(device);
        }
        
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

        private void TryStopCoroutine(ref Coroutine coroutine)
        {
            // unity may have destroyed the coroutine if it's already finished
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        public delegate void DisplayPosition(float position);
    }
}