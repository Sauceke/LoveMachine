﻿using LoveMachine.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoveMachine.VRK
{
    internal class VRKanojoGame : GameDescriptor
    {
        private CharFemale sakura;

        public void OnStartH(VK_H_Houshi_Sonyu hscene)
        {
            sakura = hscene.chaFemale;
            StartH();
        }

        public override int AnimationLayer => 0;

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "cf_J_Kosi01_s" },
            { Bone.RightHand, "cf_J_Hand_s_R" },
            { Bone.LeftHand, "cf_J_Hand_s_L" },
            { Bone.RightBreast, "cf_J_Mune_Nip01_s_R" },
            { Bone.LeftBreast, "cf_J_Mune_Nip01_s_L" }
        };

        protected override int HeroineCount => 1;

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => false;

        protected override bool IsHSceneInterrupted => false;

        public override Animator GetFemaleAnimator(int girlIndex) => sakura.animBody;

        protected override Transform GetDickBase() => GameObject.Find("cm_J_dan_s").transform;

        protected override GameObject GetFemaleRoot(int girlIndex) => null;

        protected override string GetPose(int girlIndex) => sakura.nowMotionName;

        protected override bool IsIdle(int girlIndex) =>
            sakura.nowMotionName == "Idle" || sakura.nowMotionName.EndsWith("_A");

        protected override bool IsOrgasming(int girlIndex) =>
            sakura.nowMotionName.StartsWith("Orgasm") && !sakura.nowMotionName.EndsWith("_A");

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSeconds(5f);
        }
    }
}