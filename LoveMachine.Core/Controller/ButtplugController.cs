using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace LoveMachine.Core
{
    public abstract class ButtplugController : CoroutineHandler
    {
        protected ButtplugWsClient client;
        protected GameDescriptor game;
        protected AnimationAnalyzer analyzer;

        protected abstract bool IsDeviceSupported(Device device);

        protected abstract IEnumerator Run(Device device);

        protected virtual IEnumerator Run()
        {
            foreach (var device in client.Devices.Where(IsDeviceSupported))
            {
                CoreConfig.Logger.LogInfo($"Running controller {GetType().Name} " +
                    $"on device #{device.DeviceIndex} ({device.DeviceName}).");
                HandleCoroutine(Run(device));
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

        private void Restart()
        {
            if (game.IsHSceneRunning)
            {
                OnEndH();
                OnStartH();
            }
        }

        private void OnDestroy() => StopAllCoroutines();

        protected void NerfAnimationSpeeds(float animStrokeTimeSecs, params Animator[] animators)
        {
            float speedMultiplier =
                Math.Min(1, animStrokeTimeSecs * StrokerConfig.MaxStrokesPerMinute.Value / 60f);
            foreach (var animator in animators)
            {
                animator.speed = Mathf.Min(animator.speed, speedMultiplier);
            }
        }
    }
}
