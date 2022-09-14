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
            normalizedTime = Time.time / duration.Value;
            length = duration.Value;
            speed = 1f;
        }

        protected override IEnumerator UntilReady()
        {
            var timeline = Type.GetType("Timeline.Timeline, Timeline");
            isPlaying = Traverse.Create(timeline).Property<bool>(nameof(isPlaying));
            duration = Traverse.Create(timeline).Property<float>(nameof(duration));
            yield break;
        }
    }
}
