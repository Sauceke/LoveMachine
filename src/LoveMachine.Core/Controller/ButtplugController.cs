using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Config;
using LoveMachine.Core.Game;
using LoveMachine.Core.NonPortable;
using UnityEngine;

namespace LoveMachine.Core.Controller
{
    internal abstract class ButtplugController : CoroutineHandler
    {
        private readonly Dictionary<DeviceFeature, float> normalizedLatencies =
            new Dictionary<DeviceFeature, float>();
        
        private AnimationAnalyzer analyzer;
        
        [HideFromIl2Cpp]
        protected ButtplugWsClient Client { get; private set; }
        
        [HideFromIl2Cpp]
        protected GameAdapter Game { get; private set; }
        
        [HideFromIl2Cpp]
        public abstract string FeatureName { get; }

        public abstract Buttplug.Buttplug.Feature[] GetSupportedFeatures(Device device);

        protected abstract IEnumerator Run(DeviceFeature feature);

        public bool IsDeviceSupported(Device device) => GetSupportedFeatures(device).Any();

        private void Start()
        {
            Client = GetComponent<ButtplugWsClient>();
            Game = GetComponent<GameAdapter>();
            analyzer = GetComponent<AnimationAnalyzer>();
            Game.OnHStarted += (s, a) => OnStartH();
            Game.OnHEnded += (s, a) => OnEndH();
            Client.OnDeviceListUpdated += (s, a) => Restart();
        }

        private void OnStartH() => HandleCoroutine(Run());

        private void OnEndH()
        {
            StopAllCoroutines();
            Client.StopAllDevices();
        }

        private void Restart()
        {
            if (Game.IsHSceneRunning)
            {
                OnEndH();
                OnStartH();
            }
        }

        private void OnDestroy() => StopAllCoroutines();

        private IEnumerator Run()
        {
            foreach (var device in Client.Devices.Where(IsDeviceSupported))
            {
                foreach (var feature in GetSupportedFeatures(device))
                {
                    var deviceFeature = new DeviceFeature(device, feature);
                    Logger.LogInfo($"Running controller {GetType().Name} on feature" +
                        $"{feature.ActuatorType} (#{deviceFeature.FeatureIndex}) of device" +
                        $"{device.DeviceName} (#{device.DeviceIndex}).");
                    HandleCoroutine(Run(deviceFeature));
                    HandleCoroutine(RunLatencyUpdateLoop(deviceFeature));
                }
            }
            yield break;
        }
        
        private IEnumerator RunLatencyUpdateLoop(DeviceFeature feature)
        {
            while (true)
            {
                // updating the latency in real time causes a lot of stutter when
                // there's a gradual change in animation speed
                // updating every 3s and caching the result solves this
                yield return new WaitForSecondsRealtime(3f);
                float animTimeSecs = Game.GetAnimationTimeSecs(feature.Settings.GirlIndex);
                normalizedLatencies[feature] = feature.Device.Settings.LatencyMs / 1000f / animTimeSecs;
            }
        }

        private float GetLatencyAndPhaseCorrectedNormalizedTime(DeviceFeature feature)
        {
            if (!normalizedLatencies.TryGetValue(feature, out float normalizedLatency))
            {
                normalizedLatency = 0f;
            }
            Game.GetAnimState(feature.Settings.GirlIndex, out float currentNormTime, out _, out _);
            return currentNormTime + normalizedLatency + feature.Settings.PhaseShift;
        }

        protected virtual bool TryGetCurrentStrokeInfo(DeviceFeature feature, out StrokeInfo result)
        {
            var girlIndex = feature.Settings.GirlIndex;
            var bone = feature.Settings.Bone;
            float normalizedTime = GetLatencyAndPhaseCorrectedNormalizedTime(feature);
            var trackingKey = new TrackingKey
            {
                GirlIndex = girlIndex,
                Bone = bone,
                Pose = Game.GetPose(girlIndex),
                POV = CoreConfig.POV.Value,
                MovementType = feature.Settings.MovementType,
                Axis = feature.Settings.Axis
            };
            return analyzer.TryGetCurrentStrokeInfo(trackingKey, normalizedTime, out result);
        }
        
        /// <summary>
        /// Converts the given seconds to in-game time if possible, and waits that long. <br/>
        /// WARNING: THIS WILL STILL HANG IF THE GAME IS PAUSED DURING THE YIELD! <br/>
        /// Why use it then: more in-sync with the game than Realtime, and probably more efficient
        /// </summary>
        protected object WaitForSecondsUnscaled(float seconds) => Time.timeScale > 0f
            ? (object)new WaitForSeconds(seconds * Time.timeScale)
            : new WaitForSecondsRealtime(seconds);
    }
}