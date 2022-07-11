using System;
using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public abstract class ButtplugController : CoroutineHandler
    {
        protected ButtplugWsClient client;

        protected GameDescriptor game;

        protected AnimationAnalyzer analyzer;

        public void Start()
        {
            client = gameObject.GetComponent<ButtplugWsClient>();
            game = gameObject.GetComponent<GameDescriptor>();
            analyzer = gameObject.GetComponent<AnimationAnalyzer>();
            game.OnHStarted += (s, a) => OnStartH();
            game.OnHEnded += (s, a) => OnEndH();
        }

        public void OnStartH() => StartCoroutine(RunLoops());

        public void OnEndH()
        {
            StopAllCoroutines();
            for (int girlIndex = 0; girlIndex < game.HeroineCount; girlIndex++)
            {
                analyzer.StopAnalyze(girlIndex);
                foreach (var bone in game.GetSupportedBones(girlIndex))
                {
                    StopDevices(girlIndex, bone);
                }
            }
            analyzer.ClearCache();
        }

        private IEnumerator RunLoops()
        {
            yield return HandleCoroutine(game.UntilReady());
            for (int girlIndex = 0; girlIndex < game.HeroineCount; girlIndex++)
            {
                analyzer.StartAnalyze(girlIndex);
                foreach (var bone in game.GetSupportedBones(girlIndex))
                {
                    CoreConfig.Logger.LogInfo("Starting monitoring loop in controller " +
                        $"{GetType().Name} for girl index {girlIndex} and bone {bone}. ");
                    HandleCoroutine(Run(girlIndex, bone));
                }
            }
            yield return new WaitUntil(() => game.IsHSceneInterrupted);
            OnEndH();
        }

        protected abstract IEnumerator Run(int girlIndex, Bone bone);

        protected abstract void StopDevices(int girlIndex, Bone bone);

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
