using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Bootstrap;
using UnityEngine;

namespace LoveMachine.Core
{
    public abstract class ButtplugController : CoroutineHandler
    {
        protected ButtplugWsClient client;
        protected GameDescriptor game;
        protected AnimationAnalyzer analyzer;
        private readonly Dictionary<Device, float> normalizedLatencies =
            new Dictionary<Device, float>();

        protected abstract bool IsDeviceSupported(Device device);

        protected abstract IEnumerator Run(Device device);

        protected virtual IEnumerator Run()
        {
            foreach (var device in client.Devices.Where(IsDeviceSupported))
            {
                CoreConfig.Logger.LogInfo($"Running controller {GetType().Name} " +
                    $"on device #{device.DeviceIndex} ({device.DeviceName}).");
                HandleCoroutine(Run(device));
                HandleCoroutine(RunLatencyUpdateLoop(device));
            }
            yield break;
        }

        protected virtual void Start()
        {
            client = gameObject.GetComponent<ButtplugWsClient>();
            game = gameObject.GetComponent<GameDescriptor>();
            analyzer = gameObject.GetComponent<AnimationAnalyzer>();
            game.OnHStarted += (s, a) => OnStartH();
            game.OnHEnded += (s, a) => OnEndH();
            client.OnDeviceListUpdated += (s, args) => Restart();
        }

        public void OnStartH() => HandleCoroutine(RunLoops());

        public void OnEndH()
        {
            StopAllCoroutines();
            client.StopAllDevices();
        }

        private IEnumerator RunLoops()
        {
            yield return HandleCoroutine(game.UntilReady());
            HandleCoroutine(Run());
            yield return new WaitUntil(() => game.IsHSceneInterrupted);
            OnEndH();
        }

        public static void Test<T>(Device device)
            where T : ButtplugController
        {
            var testController = Chainloader.ManagerObject.AddComponent<T>();
            testController.HandleCoroutine(testController.RunTest(device));
        }

        private IEnumerator RunTest(Device device)
        {
            yield return new WaitForEndOfFrame();
            var testGame = new TestGame();
            game = testGame;
            analyzer = new TestAnimationAnalyzer();
            if (IsDeviceSupported(device))
            {
                var test = HandleCoroutine(Run(device));
                yield return HandleCoroutine(testGame.RunTest(strokes: 3, strokesPerSec: 1f));
                yield return HandleCoroutine(testGame.RunTest(strokes: 5, strokesPerSec: 3f));
                StopCoroutine(test);
                client.StopAllDevices();
            }
            Destroy(this);
        }

        private void Restart()
        {
            if (game.IsHSceneRunning)
            {
                OnEndH();
                OnStartH();
            }
        }

        private void OnDestroy() => StopAllCoroutines();

        private IEnumerator RunLatencyUpdateLoop(Device device)
        {
            while (true)
            {
                // updating the latency in real time causes a lot of stutter when
                // there's a gradual change in animation speed
                // updating every 3s and caching the result solves this
                yield return new WaitForSecondsRealtime(3f);
                float animTimeSecs = GetAnimationTimeSecs(device.Settings.GirlIndex);
                float normalizedLatency = device.Settings.LatencyMs / 1000f / animTimeSecs;
                normalizedLatencies[device] = normalizedLatency;
            }
        }

        protected float GetLatencyCorrectedNormalizedTime(Device device)
        {
            if (!normalizedLatencies.TryGetValue(device, out float normalizedLatency))
            {
                normalizedLatency = 0f;
            }
            game.GetAnimState(device.Settings.GirlIndex, out float currentNormTime, out _, out _);
            return currentNormTime + normalizedLatency;
        }

        protected float GetAnimationTimeSecs(int girlIndex)
        {
            game.GetAnimState(girlIndex, out _, out float length, out float speed);
            float strokeTimeSecs = length / speed;
            // sometimes the length of an animation becomes Infinity in KK
            // sometimes the speed becomes 0 in HS2
            // this is a catch-all for god knows what other things that can
            // possibly go wrong and cause the stroking coroutine to hang
            if (strokeTimeSecs > 10 || strokeTimeSecs < 0.001f
                || float.IsNaN(strokeTimeSecs))
            {
                return .01f;
            }
            return strokeTimeSecs;
        }
    }
}
