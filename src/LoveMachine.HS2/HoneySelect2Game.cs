using IllusionUtility.GetUtility;
using LoveMachine.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoveMachine.HS2
{
    internal sealed class HoneySelect2Game : GameDescriptor
    {
        private HScene hScene;

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "cf_J_Kokan" },
            { Bone.RightHand, "cf_J_Hand_Wrist_s_R" },
            { Bone.LeftHand, "cf_J_Hand_Wrist_s_L" },
            { Bone.RightBreast, "cf_J_Mune04_s_R" },
            { Bone.LeftBreast, "cf_J_Mune04_s_L" },
            { Bone.Mouth, "cf_J_MouthCavity" },
            { Bone.RightFoot, "cf_J_Toes01_L" },
            { Bone.LeftFoot, "cf_J_Toes01_R" }
        };

        protected override int HeroineCount =>
            Array.FindAll(hScene.GetFemales(), f => f != null).Length;

        protected override int MaxHeroineCount => 2;

        protected override bool IsHardSex => hScene.ctrlFlag.loopType == 1;

        public override int AnimationLayer => 0;

        protected override float PenisSize => 0.4f;

        public override Animator GetFemaleAnimator(int girlIndex) =>
            hScene?.GetFemales()[girlIndex]?.animBody;

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            hScene.GetFemales()[girlIndex].objBodyBone;

        protected override Transform GetDickBase() =>
            hScene.GetMales()[0].objBodyBone.transform.FindLoop("cm_J_dan_f_L").transform;

        public void StartH(HScene scene)
        {
            hScene = scene;
            StartH();
        }

        protected override string GetPose(int girlIndex) =>
            // couldn't find accessor for animation name so going with hash
            hScene.ctrlFlag.nowAnimationInfo.id
                + "." + hScene.GetFemales()[girlIndex].getAnimatorStateInfo(0).fullPathHash;

        protected override IEnumerator UntilReady()
        {
            yield return new WaitWhile(() => hScene.GetFemales().Length == 0
                || hScene.GetFemales()[0] == null
                || hScene.GetMales().Length == 0
                || hScene.GetMales()[0] == null);
        }

        protected override bool IsIdle(int girlIndex) => hScene.ctrlFlag.loopType == -1;

        protected override bool IsOrgasming(int girlIndex) => hScene.ctrlFlag.nowOrgasm;
    }
}