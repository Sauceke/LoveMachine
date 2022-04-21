using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IllusionUtility.GetUtility;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.HS2
{
    internal abstract class HoneySelect2ButtplugController : ButtplugController
    {
        private const string MaleBoneName = "cm_J_dan_f_L"; // left testicle

        private static readonly Dictionary<Bone, string> femaleBones
            = new Dictionary<Bone, string>
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

        private static readonly string[] idleAnimations =
        {
            "Idle", "WIdle", "SIdle", "Insert", "D_Idle", "D_Insert",
            "Orgasm_A", "Orgasm_IN_A", "Orgasm_OUT_A", "Drink_A", "Vomit_A", "OrgasmM_OUT_A",
            "D_Orgasm_A", "D_Orgasm_OUT_A", "D_Orgasm_IN_A", "D_OrgasmM_OUT_A"
        };

        protected HScene hScene;

        protected override int HeroineCount
            => Array.FindAll(hScene.GetFemales(), f => f != null).Length;

        protected override bool IsHardSex
            => GetFemaleAnimator(0)?.GetCurrentAnimatorStateInfo(0).IsName("SLoop") ?? false;

        protected override int AnimationLayer => 0;

        protected override bool IsHSceneInterrupted => false;

        protected override float PenisSize => 0.4f;

        protected override Animator GetFemaleAnimator(int girlIndex)
            => hScene?.GetFemales()[girlIndex]?.animBody;

        protected override Dictionary<Bone, Transform> GetFemaleBones(int girlIndex)
        {
            var bodyBone = hScene.GetFemales()[girlIndex].objBodyBone.transform;
            return femaleBones.ToDictionary(kvp => kvp.Key, kvp => bodyBone.FindLoop(kvp.Value));
        }

        protected override Transform GetMaleBone()
        {
            var bodyBone = hScene.GetMales()[0].objBodyBone.transform;
            return bodyBone.FindLoop(MaleBoneName).transform;
        }

        public void OnStartH(HScene scene)
        {
            hScene = scene;
            OnStartH();
        }

        protected override string GetPose(int girlIndex)
        {
            // couldn't find accessor for animation name so going with hash
            return hScene.ctrlFlag.nowAnimationInfo.id
                + "." + hScene.GetFemales()[girlIndex].getAnimatorStateInfo(0).fullPathHash;
        }

        protected override IEnumerator UntilReady()
        {
            while (hScene.GetFemales().Length == 0
                || hScene.GetFemales()[0] == null
                || hScene.GetMales().Length == 0
                || hScene.GetMales()[0] == null)
            {
                yield return new WaitForSeconds(.1f);
            }
        }

        protected override bool IsIdle(int girlIndex) => idleAnimations.Any(
            name => GetFemaleAnimator(girlIndex).GetCurrentAnimatorStateInfo(0).IsName(name));
    }

    internal class HoneySelect2ButtplugVibrationController : HoneySelect2ButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone)
            => RunVibratorLoop(girlIndex, bone);
    }

    internal class HoneySelect2ButtplugStrokerController : HoneySelect2ButtplugController
    {
        private static readonly string[] orgasmAnimations =
        {
            "Orgasm", "Orgasm_IN", "Orgasm_OUT", "Drink", "Vomit", "OrgasmM_OUT",
            "D_Orgasm", "D_Orgasm_OUT", "D_Orgasm_IN", "D_OrgasmM_OUT"
        };

        protected override bool IsOrgasming(int girlIndex)
        {
            var anim = GetFemaleAnimator(girlIndex);
            return orgasmAnimations.Any(name => anim.GetCurrentAnimatorStateInfo(0).IsName(name));
        }

        protected override IEnumerator Run(int girlIndex, Bone bone) =>
            RunStrokerLoop(girlIndex, bone);
    }
}
