using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IllusionUtility.GetUtility;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.HS2
{
    internal sealed class HoneySelect2Game : GameDescriptor
    {
        private static readonly string[] idleAnimations =
        {
            "Idle", "WIdle", "SIdle", "Insert", "D_Idle", "D_Insert",
            "Orgasm_A", "Orgasm_IN_A", "Orgasm_OUT_A", "Drink_A", "Vomit_A", "OrgasmM_OUT_A",
            "D_Orgasm_A", "D_Orgasm_OUT_A", "D_Orgasm_IN_A", "D_OrgasmM_OUT_A"
        };

        private static readonly string[] orgasmAnimations =
        {
            "Orgasm", "Orgasm_IN", "Orgasm_OUT", "Drink", "Vomit", "OrgasmM_OUT", "OrgasmM_IN",
            "D_Orgasm", "D_Orgasm_OUT", "D_Orgasm_IN", "D_OrgasmM_OUT", "D_OrgasmM_IN"
        };

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

        protected override bool IsHardSex =>
            GetFemaleAnimator(0)?.GetCurrentAnimatorStateInfo(0).IsName("SLoop") ?? false;

        public override int AnimationLayer => 0;

        protected override bool IsHSceneInterrupted => false;

        protected override float PenisSize => 0.4f;

        public override Animator GetFemaleAnimator(int girlIndex) =>
            hScene?.GetFemales()[girlIndex]?.animBody;

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            hScene.GetFemales()[girlIndex].objBodyBone;

        protected override Transform GetDickBase() =>
            hScene.GetMales()[0].objBodyBone.transform.FindLoop("cm_J_dan_f_L").transform;

        public void OnStartH(HScene scene)
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

        protected override bool IsIdle(int girlIndex) => 
            idleAnimations.Any(GetAnimatorStateInfo(girlIndex).IsName);

        protected override bool IsOrgasming(int girlIndex) =>
            orgasmAnimations.Any(GetAnimatorStateInfo(girlIndex).IsName);
    }
}
