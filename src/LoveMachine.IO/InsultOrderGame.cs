using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.IO
{
    internal sealed class InsultOrderGame : GameDescriptor
    {
        private GameObject femaleRoot;
        private Traverse<bool> climax;

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "HS01_cli" },
            { Bone.LeftHand, "bip01 L Finger1Nub" },
            { Bone.RightHand, "bip01 R Finger1Nub" },
            { Bone.Mouth, "HF01_tongue01" },
            { Bone.LeftBreast, "HS_Breast_LL" },
            { Bone.RightBreast, "HS_Breast_RR" },
            { Bone.LeftFoot, "bip01 L Toe0Nub" },
            { Bone.RightFoot, "bip01 R Toe0Nub" },
        };

        protected override int HeroineCount => 1;

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => false;

        protected override bool IsHSceneInterrupted => false;

        protected override float PenisSize => 0.5f;

        public override int AnimationLayer => 0;

        public override Animator GetFemaleAnimator(int girlIndex) =>
            GetFemaleRoot(girlIndex)?.GetComponent<Animator>();

        protected override GameObject GetFemaleRoot(int girlIndex) => femaleRoot;

        protected override Transform GetDickBase() => GameObject.Find("BP00_tamaL").transform;

        protected override string GetPose(int girlIndex) =>
            GetFemaleAnimator(girlIndex).GetCurrentAnimatorClipInfo(0)[0].clip.name;

        protected override bool IsIdle(int girlIndex) => GetFemaleAnimator(girlIndex) == null;

        protected override bool IsOrgasming(int girlIndex) => climax.Value;

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSeconds(5f);
            femaleRoot = GameObject.Find("CH01/CH0001") ?? GameObject.Find("CH02/CH0002");
            climax = Traverse.Create(Type.GetType("GameClass, Assembly-CSharp"))
                .Field<bool>("Climax");
        }
    }
}
