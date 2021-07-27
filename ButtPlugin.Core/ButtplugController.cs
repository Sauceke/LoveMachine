using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ButtPlugin.Core
{
    public abstract class ButtplugController : MonoBehaviour
    {
        protected ButtplugWsClient client;
        
        // animation -> fractional part of normalized time at start of up-stroke
        protected Dictionary<string, float> animPhases;

        protected float GetPhase(int girlIndex = 0)
        {
            animPhases.TryGetValue(GetPose(girlIndex), out float phase);
            return phase;
        }

        public bool IsFemale
        {
            get { return (CoreConfig.EnableVibrate.Value & VibrationMode.Female) == VibrationMode.Female; }
        }

        public bool IsMale
        {
            get { return (CoreConfig.EnableVibrate.Value & VibrationMode.Male) == VibrationMode.Male; }
        }

        public void Awake()
        {
            client = gameObject.GetComponent<ButtplugWsClient>();
            string animConfigPath = Path.GetDirectoryName(CoreConfig.Info.Location)
                + Path.DirectorySeparatorChar
                + AnimConfigJsonName;
            string animConfigJson = File.ReadAllText(animConfigPath);
            animPhases = JsonMapper.ToObject<Dictionary<string, float>>(animConfigJson);
        }

        public void OnStartH()
        {
            StartCoroutine(RunLoops());
        }

        IEnumerator RunLoops()
        {
            yield return StartCoroutine(UntilReady());
            for (int i = 0; i < HeroineCount; i++)
            {
                StartCoroutine(HandleExceptions(Run(girlIndex: i)));
            }
        }

        IEnumerator HandleExceptions(IEnumerator coroutine)
        {
            while (TryNext(coroutine))
            {
                yield return coroutine.Current;
            }
        }

        private bool TryNext(IEnumerator coroutine)
        {
            try
            {
                return coroutine.MoveNext();
            }
            catch (Exception e)
            {
                CoreConfig.Logger.LogError($"Coroutine failed with exception: {e}");
                throw e;
            }
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }

        protected void NerfAnimationSpeeds(float animStrokeTimeSecs, params Animator[] animators)
        {
            float speedMultiplier =
                Math.Min(1, animStrokeTimeSecs * CoreConfig.MaxStrokesPerMinute.Value / 60f);
            foreach (var animator in animators)
            {
                animator.speed = speedMultiplier;
            }
        }

        protected float GetStrokeTimeSecs(AnimatorStateInfo info)
        {
            return info.length / info.speed;
        }

        protected IEnumerator DoStroke(float strokeTimeSecs, int girlIndex)
        {
            // sometimes the length of an animation becomes Infinity in KK
            // this is a catch-all for god knows what other things that can
            // possibly go wrong and cause this coroutine to hang
            if (strokeTimeSecs > 10)
            {
                yield break;
            }
            int strokeTimeMs = (int)(strokeTimeSecs * 1000) - 10;
            // decrease stroke length gradually as speed approaches the device limit
            double rate = 60f / CoreConfig.MaxStrokesPerMinute.Value / strokeTimeSecs;
            double margin = rate * rate * 0.3;
            client.LinearCmd(
                position: 1 - margin * 0.7,
                durationMs: strokeTimeMs / 2,
                girlIndex);
            yield return new WaitForSeconds(strokeTimeSecs / 2f);
            client.LinearCmd(
                position: margin * 0.3,
                durationMs: strokeTimeMs / 2,
                girlIndex);
        }

        protected IEnumerator WaitForUpStroke(Func<AnimatorStateInfo> info, int girlIndex)
        {
            var initialState = info();
            float startNormTime = initialState.normalizedTime;
            float phase = GetPhase(girlIndex);
            float strokeTimeSecs = initialState.length / initialState.speed;
            float latencyNormTime = CoreConfig.LatencyMs.Value / 1000f / strokeTimeSecs;
            phase -= latencyNormTime;
            while ((int)(info().normalizedTime - phase + 2) == (int)(startNormTime - phase + 2))
            {
                yield return new WaitForSeconds(.01f);
            }
        }

        protected float GetVibrationStrength(AnimatorStateInfo info, int girlIndex)
        {
            if (CoreConfig.SyncVibrationWithAnimation.Value)
            {
                // Simple cos based intensity amplification based on normalized position in looping animation
                float depth = (info.normalizedTime - GetPhase(girlIndex)) % 1;
                return Mathf.Abs(Mathf.Cos(Mathf.PI * depth)) + 0.1f;
            }
            return 1f;
        }

        protected abstract string AnimConfigJsonName { get; }
        protected abstract int HeroineCount { get; }
        protected abstract string GetPose(int girlIndex = 0);
        protected abstract IEnumerator UntilReady();
        protected abstract IEnumerator Run(int girlIndex);

        public enum VibrationMode
        {
            Off = 0_00,
            Male = 0_01,
            Female = 0_10,
            Both = 0_11
        }
    }
}
