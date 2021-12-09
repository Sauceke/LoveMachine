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

        internal static readonly Dictionary<string, string> femaleBones
            = new Dictionary<string, string>
        {
                { "cf_J_Kokan", "Pussy" },
                { "cf_J_Hand_Wrist_s_R", "Right Hand" },
                { "cf_J_Hand_Wrist_s_L", "Left Hand" },
                { "cf_J_Mune04_s_R", "Right Breast" },
                { "cf_J_Mune04_s_L", "Left Breast" },
                { "cf_J_MouthCavity", "Mouth" },
                { "cf_J_Toes01_L", "Right Foot" },
                { "cf_J_Toes01_R", "Left Foot" }
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

        protected override List<Transform> GetFemaleBones(int girlIndex)
        {
            var bodyBone = hScene.GetFemales()[girlIndex].objBodyBone.transform;
            return femaleBones.Keys.Select(name => bodyBone.FindLoop(name).transform).ToList();
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

        protected override int GetStrokesPerAnimationCycle(int girlIndex)
            => IsOrgasm(GetFemaleAnimator(girlIndex)) ? 2 : 1;

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
        protected override IEnumerator Run(int girlIndex, int boneIndex)
            => RunVibratorLoop(girlIndex, boneIndex);
    }

    public class HoneySelect2ButtplugStrokerController : HoneySelect2ButtplugController
    {
        protected override IEnumerator Run(int girlIndex, int boneIndex)
            => RunStrokerLoop(girlIndex, boneIndex);
    }
}
