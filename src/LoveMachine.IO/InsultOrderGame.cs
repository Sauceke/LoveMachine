using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Game;
using LoveMachine.Core.Common;
using UnityEngine;

namespace LoveMachine.IO
{
    internal sealed class InsultOrderGame : GameAdapter
    {
        private static readonly string[] idleMotions = { "", "H1", "H2" };

        private GameObject femaleRoot;
        private Animator femaleAnimator;
        private Traverse<bool> femaleClimax;
        private Traverse<bool> maleClimax;
        private Traverse<string> motionId;

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "HS01_cli" },
            { Bone.LeftHand, "bip01 L Finger1Nub" },
            { Bone.RightHand, "bip01 R Finger1Nub" },
            { Bone.Mouth, "HF01_tongue01" },
            { Bone.LeftBreast, "HS_Breast_LL" },
            { Bone.RightBreast, "HS_Breast_RR" },
            { Bone.LeftFoot, "bip01 L Toe0Nub" },
            { Bone.RightFoot, "bip01 R Toe0Nub" }
        };

        protected override int HeroineCount => 1;

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => false;

        protected override float PenisSize => 0.5f;

        protected override float MinOrgasmDurationSecs => 0.5f;

        protected override int AnimationLayer => 0;

        protected override MethodInfo[] StartHMethods =>
            new[] { AccessTools.Method("FH_AnimeController, Assembly-CSharp:Start") };

        protected override MethodInfo[] EndHMethods =>
            new[] { AccessTools.Method("FH_SetUp, Assembly-CSharp:Unload") };

        protected override Animator GetFemaleAnimator(int girlIndex) => femaleAnimator;

        protected override GameObject GetFemaleRoot(int girlIndex) => femaleRoot;

        protected override Transform PenisBase => GameObject.Find("BP00_tamaL").transform;

        protected override string GetPose(int girlIndex) =>
            femaleAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        protected override bool IsIdle(int girlIndex) => idleMotions.Contains(motionId.Value);

        protected override bool IsOrgasming(int girlIndex) =>
            femaleClimax.Value || maleClimax.Value;

        protected override void OnStartH(object animeController) =>
            motionId = Traverse.Create(animeController).Field<string>("MotionID");

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSeconds(5f);
            femaleRoot = GameObject.Find("CH01/CH0001") ?? GameObject.Find("CH02/CH0002");
            femaleAnimator = femaleRoot.GetComponent<Animator>();
            femaleClimax = Traverse.Create(Type.GetType("GameClass, Assembly-CSharp"))
                .Field<bool>("Climax");
            maleClimax = Traverse.Create(FindObjectOfType(Type.GetType(
                    "SE_Particle_Manager, Assembly-CSharp")))
                .Field<bool>("SE0101touch");
        }
    }
}