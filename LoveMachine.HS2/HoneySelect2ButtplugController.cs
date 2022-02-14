using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IllusionUtility.GetUtility;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.HS2
{
    public abstract class HoneySelect2ButtplugController : ButtplugController
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

        private static readonly List<string> idleAnimations = new List<string>
        {
            "Idle", "WIdle", "SIdle", "Insert", "D_Idle", "D_Insert",
            "Orgasm_A", "Orgasm_IN_A", "Orgasm_OUT_A", "Drink_A", "Vomit_A", "OrgasmM_OUT_A",
            "D_Orgasm_A", "D_Orgasm_OUT_A", "D_Orgasm_IN_A", "D_OrgasmM_OUT_A"
        };

        private static readonly List<string> orgasmAnimations = new List<string>
        {
            "OLoop", "D_OLoop"
        };

        protected HScene hScene;

        protected override int HeroineCount
            => Array.FindAll(hScene.GetFemales(), f => f != null).Length;

        protected override bool IsHardSex
            => GetFemaleAnimator(0)?.GetCurrentAnimatorStateInfo(0).IsName("SLoop") ?? false;

        protected override int AnimationLayer => 0;

        protected override bool IsHSceneInterrupted => false;

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
                + "." + hScene.GetFemales()[girlIndex].getAnimatorStateInfo(0).fullPathHash
                + "." + girlIndex;
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

        protected bool IsOrgasm(Animator animator)
            => orgasmAnimations.Any(name => animator.GetCurrentAnimatorStateInfo(0).IsName(name));
    }

    public class HoneySelect2ButtplugVibrationController : HoneySelect2ButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone)
            => RunVibratorLoop(girlIndex, bone);
    }

    public class HoneySelect2ButtplugStrokerController : HoneySelect2ButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone)
            => RunStrokerLoop(girlIndex, bone);
    }
}
