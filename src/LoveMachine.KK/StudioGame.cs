using HarmonyLib;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace LoveMachine.KK
{
    internal sealed class StudioGame : AbstractKoikatsuGame
    {
        private const string balls = "chaM_001/BodyTop/p_cf_body_bone/cf_j_root/cf_n_height/" +
            "cf_j_hips/cf_j_waist01/cf_j_waist02/cf_d_kokan/cm_J_dan_top/cm_J_dan_f_top/" +
            "cm_J_dan_f_L/k_f_tamaL_00";

        private Traverse<bool> isPlaying;
        private Traverse<float> duration;
        private Traverse<float> playbackTime;

        public override int AnimationLayer => throw new NotImplementedException();

        protected override int HeroineCount => 1;

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => false;

        protected override MethodInfo[] StartHMethods =>
            new[] { AccessTools.Method(typeof(Studio.Studio), nameof(Studio.Studio.InitScene)) };

        protected override MethodInfo[] EndHMethods => new MethodInfo[] { };

        public override Animator GetFemaleAnimator(int girlIndex) =>
            throw new NotImplementedException();

        protected override GameObject GetFemaleRoot(int girlIndex) => GameObject.Find("chaF_001");

        protected override Transform GetDickBase() => GameObject.Find(balls).transform;

        protected override string GetPose(int girlIndex) =>
            Studio.Studio.Instance.sceneInfo.GetHashCode().ToString() + isPlaying.Value;

        protected override bool IsIdle(int girlIndex) => !isPlaying.Value;

        protected override void GetAnimState(int girlIndex, out float normalizedTime,
            out float length, out float speed)
        {
            float offset = (Time.time - playbackTime.Value) % duration.Value;
            normalizedTime = (Time.time - offset) / duration.Value;
            length = duration.Value;
            speed = 1f;
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