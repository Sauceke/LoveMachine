using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoveMachine.Core
{
    public abstract class ButtplugController : CoroutineHandler
    {
        protected ButtplugWsClient client;
        protected GameDescriptor game;
        private AnimationAnalyzer analyzer;

        private readonly Dictionary<Device, float> normalizedLatencies =
            new Dictionary<Device, float>();

        private bool isTestController = false;

        protected abstract bool IsDeviceSupported(Device device);

        protected abstract IEnumerator Run(Device device);

        protected virtual IEnumerator Run()
        {
            foreach (var device in client.Devices.Where(IsDeviceSupported))
            {
                Logger.LogInfo($"Running controller {GetType().Name} " +
                    $"on device #{device.DeviceIndex} ({device.DeviceName}).");
                HandleCoroutine(Run(device));
                HandleCoroutine(RunLatencyUpdateLoop(device));
            }
            yield break;
        }

        protected virtual void Start()
        {
            client = GetComponent<ButtplugWsClient>();
            game = GetComponent<GameDescriptor>();
            analyzer = GetComponent<AnimationAnalyzer>();
            if (!isTestController)
            {
                game.OnHStarted += (s, a) => OnStartH();
                game.OnHEnded += (s, a) => OnEndH();
                client.OnDeviceListUpdated += (s, a) => Restart();
            }
        }

        private void OnStartH() => HandleCoroutine(Run());

        private void OnEndH()
        {
            StopAllCoroutines();
            client.StopAllDevices();
        }

        public static void Test<T>(Device device, Action<float> display)
            where T : ButtplugController
        {
            var testController = Globals.ManagerObject.AddComponent<T>();
            testController.isTestController = true;
            testController.HandleCoroutine(testController.RunTest(device, display));
        }

        private IEnumerator RunTest(Device device, Action<float> display)
        {
            yield return new WaitForEndOfFrame();
            var testGame = new TestGame();
            game = testGame;
            analyzer = new TestAnimationAnalyzer(testGame);
            if (IsDeviceSupported(device))
            {
                var test = HandleCoroutine(Run(device));
                yield return HandleCoroutine(
                    testGame.RunTest(strokes: 2, strokesPerSec: 0.5f, display));
                yield return HandleCoroutine(
                    testGame.RunTest(strokes: 2, strokesPerSec: 1f, display));
                yield return HandleCoroutine(
                    testGame.RunTest(strokes: 5, strokesPerSec: 3f, display));
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
                float animTimeSecs = game.GetAnimationTimeSecs(device.Settings.GirlIndex);
                normalizedLatencies[device] = device.Settings.LatencyMs / 1000f / animTimeSecs;
            }
        }

        private float GetLatencyCorrectedNormalizedTime(Device device)
        {
            if (!normalizedLatencies.TryGetValue(device, out float normalizedLatency))
            {
                normalizedLatency = 0f;
            }
            game.GetAnimState(device.Settings.GirlIndex, out float currentNormTime, out _, out _);
            return currentNormTime + normalizedLatency;
        }

        protected bool TryGetCurrentStrokeInfo(Device device, out StrokeInfo result)
        {
            var girlIndex = device.Settings.GirlIndex;
            var bone = device.Settings.Bone;
            float normalizedTime = GetLatencyCorrectedNormalizedTime(device);
            return analyzer.TryGetCurrentStrokeInfo(girlIndex, bone, normalizedTime, out result);
        }
    }
}