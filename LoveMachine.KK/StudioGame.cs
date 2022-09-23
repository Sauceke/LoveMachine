using System;
using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace LoveMachine.KK
{
    internal sealed class StudioGame : AbstractKoikatsuGame
    {
        private Traverse<bool> isPlaying;
        private Traverse<float> duration;
        private Traverse<float> playbackTime;

        public override int AnimationLayer => throw new NotImplementedException();

        protected override int HeroineCount => 1;

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => false;

        protected override bool IsHSceneInterrupted => false;

        public override Animator GetFemaleAnimator(int girlIndex) =>
            throw new NotImplementedException();
        protected override GameObject GetFemaleRoot(int girlIndex) => null;

        protected override Transform GetDickBase() => GameObject.Find("k_f_tamaL_00").transform;

        protected override string GetPose(int girlIndex) =>
            Studio.Studio.Instance.sceneInfo.GetHashCode().ToString() + isPlaying.Value;

        protected override bool IsIdle(int girlIndex) => !isPlaying.Value;

        protected override void GetAnimState(int girlIndex, out float normalizedTime,
            out float length, out float speed)
        {
            float offset = (Time.time - playbackTime.Value) % duration.Value;
            normalizedTime = (Time.time - offset) / duration.Value;
            length = duration.Value;
            speed = Time.timeScale;
        }

        protected override IEnumerator UntilReady()
        {
            var timeline = Traverse.Create(Type.GetType("Timeline.Timeline, Timeline"));
            isPlaying = timeline.Property<bool>(nameof(isPlaying));
            duration = timeline.Property<float>(nameof(duration));
            playbackTime = timeline.Property<float>(nameof(playbackTime));
            yield break;
        }
    }
}
