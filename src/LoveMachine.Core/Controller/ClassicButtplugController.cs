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
        protected abstract IEnumerator HandleAnimation(DeviceFeature feature, StrokeInfo strokeInfo);

        /// <summary>
        /// How to react to orgasms. <br/>
        /// This will be stopped automatically once the orgasm is over.
        /// </summary>
        protected abstract IEnumerator HandleOrgasm(DeviceFeature feature);

        /// <summary>
        /// If this device type has a (preferably safe) way to handle a given
        /// float value between 0.0 and 1.0 (level) over a set duration
        /// (durationSecs), then you can do that here.
        /// If it doesn't, then do nothing.
        /// </summary>
        protected abstract void HandleLevel(DeviceFeature feature, float level, float durationSecs);
        
        protected override IEnumerator Run(DeviceFeature feature)
        {
            foreach (var gimmick in GetComponents<Gimmick>())
            {
                Logger.LogInfo($"Running gimmick {gimmick.GetType()} for device " +
                    $"#{feature.Device.DeviceIndex} in controller {GetType()}.");
                HandleCoroutine(gimmick.Run(feature, HandleLevel, HandleStroke));
            }
            Coroutine strokeLoop = null;
            const float refreshTimeSecs = 0.3f;
            while (true)
            {
                // must be realtime to avoid hanging if the game is paused during this yield
                yield return new WaitForSecondsRealtime(refreshTimeSecs);
                if (Game.IsOrgasming(feature.Settings.GirlIndex))
                {
                    TryStopCoroutine(ref strokeLoop);
                    var orgasm = HandleCoroutine(HandleOrgasm(feature));
                    yield return WaitForSecondsUnscaled(Game.MinOrgasmDurationSecs);
                    while (Game.IsOrgasming(feature.Settings.GirlIndex))
                    {
                        yield return WaitForSecondsUnscaled(refreshTimeSecs);
                    }
                    TryStopCoroutine(ref orgasm);
                    continue;
                }
                if (IsIdleOrPaused(feature))
                {
                    TryStopCoroutine(ref strokeLoop);
                    Client.StopDeviceCmd(feature.Device);
                    while (IsIdleOrPaused(feature))
                    {
                        yield return WaitForSecondsUnscaled(refreshTimeSecs);
                    }
                    continue;
                }
                strokeLoop = strokeLoop ?? HandleCoroutine(RunStrokeLoop(feature));
            }
        }

        private IEnumerator RunStrokeLoop(DeviceFeature feature)
        {
            while (true)
            {
                if (!base.TryGetCurrentStrokeInfo(feature, out var strokeInfo))
                {
                    yield return WaitForSecondsUnscaled(0.1f);
                    continue;
                }
                // unwrap the coroutine so we can interrupt it midway
                var handleAnimation = HandleAnimation(feature, strokeInfo);
                while (handleAnimation.MoveNext())
                {
                    yield return handleAnimation.Current;
                }
            }
        }

        protected override bool TryGetCurrentStrokeInfo(DeviceFeature feature, out StrokeInfo result) =>
            throw new NotImplementedException("Do NOT use this; use the strokeInfo parameter!");

        private bool IsIdleOrPaused(DeviceFeature feature) =>
            Game.IsIdle(feature.Settings.GirlIndex) || Time.timeScale == 0f;

        public void Test(Device device, DisplayPosition display)
        {
            foreach (var feature in GetSupportedFeatures(device))
            {
                var deviceFeature = new DeviceFeature(device, feature);
                HandleCoroutine(RunTest(deviceFeature, display));
            }
        }

        private IEnumerator RunTest(DeviceFeature feature, DisplayPosition display)
        {
            yield return HandleCoroutine(EmulateStrokes(feature, 2, 2f, display));
            yield return HandleCoroutine(EmulateStrokes(feature, 2, 1f, display));
            yield return HandleCoroutine(EmulateStrokes(feature, 5, 0.3f, display));
            Client.StopDeviceCmd(feature.Device);
        }

        private IEnumerator HandleStroke(DeviceFeature feature, float durationSecs)
        {
            yield return HandleCoroutine(EmulateStrokes(feature, count: 1, durationSecs, _ => { }));
            Client.StopDeviceCmd(feature.Device);
        }
        
        private IEnumerator EmulateStrokes(DeviceFeature feature, int count, float durationSecs,
            DisplayPosition display)
        {
            float startTime = Time.unscaledTime;
            while (Time.unscaledTime < startTime + count * durationSecs)
            {
                float normTime = (Time.unscaledTime - startTime) / durationSecs;
                float completion = normTime % 1f;
                float normLatency = feature.Device.Settings.LatencyMs / 1000f / durationSecs;
                float realCompletion = (normTime + normLatency + feature.Settings.PhaseShift) % 1f;
                realCompletion = Mathf.Clamp(realCompletion, 0f, 1f);
                var strokeInfo = new StrokeInfo
                {
                    Amplitude = 1f,
                    Completion = realCompletion,
                    DurationSecs = durationSecs
                };
                float position = (completion < 0.5f ? completion : 1f - completion) * 2f;
                display(position);
                yield return HandleCoroutine(HandleAnimation(feature, strokeInfo));
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