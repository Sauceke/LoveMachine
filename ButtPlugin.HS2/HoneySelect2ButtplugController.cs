using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ButtPlugin.Core;
using UnityEngine;
using IllusionUtility.GetUtility;

namespace ButtPlugin.HS2
{
    public abstract class HoneySelect2ButtplugController : ButtplugController
    {
        private static readonly List<string> femaleBoneNames = new List<string>
        {
            "cf_J_Kokan", // pussy
            "cf_J_Hand_Wrist_s_R", "cf_J_Hand_Wrist_s_L",
            "cf_J_Mune04_s_R", // right nipple
            "cf_J_MouthCavity"
        };

        private static readonly List<string> maleBoneNames = new List<string>
        {
            "cm_J_dan_f_L" // left testicle
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

        protected override int HeroineCount
            => Array.FindAll(hScene.GetFemales(), f => f != null).Length;

        protected override int AnimationLayer => 0;

        protected override int CurrentAnimationStateHash
            => hScene.GetFemales()[0].animBody.GetCurrentAnimatorStateInfo(0).fullPathHash;

        protected override Animator GetFemaleAnimator(int girlIndex) => hScene.GetFemales()[girlIndex].animBody;

        protected override Animator GetMaleAnimator() => hScene.GetMales()[0].animBody;

        protected override List<Transform> GetFemaleBones(int girlIndex)
        {
            var bodyBone = hScene.GetFemales()[girlIndex].objBodyBone.transform;
            return femaleBoneNames.Select(name => bodyBone.FindLoop(name).transform).ToList();
        }

        protected override List<Transform> GetMaleBones()
        {
            var bodyBone = hScene.GetMales()[0].objBodyBone.transform;
            return maleBoneNames.Select(name => bodyBone.FindLoop(name).transform).ToList();
        }

        protected HScene hScene;

        public void OnStartH(HScene scene)
        {
            hScene = scene;
            OnStartH();
        }

        public void OnEndH()
        {
            StopAllCoroutines();
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

        protected bool IsIdle(Animator animator)
        {
            return idleAnimations.Any(
                name => animator.GetCurrentAnimatorStateInfo(0).IsName(name));
        }

        protected bool IsOrgasm(Animator animator)
        {
            return orgasmAnimations.Any(
                name => animator.GetCurrentAnimatorStateInfo(0).IsName(name));
        }
    }

    public class HoneySelect2ButtplugVibrationController : HoneySelect2ButtplugController
    {
        protected override IEnumerator Run(int girlIndex)
        {
            while (true)
            {
                AnimatorStateInfo info = hScene.GetFemales()[girlIndex].getAnimatorStateInfo(0);
                DoVibrate(GetVibrationStrength(info, girlIndex), girlIndex);
                yield return new WaitForSecondsRealtime(1.0f / CoreConfig.VibrationUpdateFrequency.Value);
            }
        }
    }

    public class HoneySelect2ButtplugStrokerController : HoneySelect2ButtplugController
    {
        protected override IEnumerator Run(int girlIndex)
        {
            var femaleAnimator = GetFemaleAnimator(girlIndex);
            while (true)
            {
                if (IsIdle(femaleAnimator))
                {
                    yield return new WaitForSeconds(.1f);
                    continue;
                }
                AnimatorStateInfo info() => hScene.GetFemales()[girlIndex].getAnimatorStateInfo(0);
                yield return HandleCoroutine(WaitForUpStroke(info, girlIndex));
                float strokeTimeSecs = GetStrokeTimeSecs(info());
                if (IsOrgasm(femaleAnimator))
                {
                    // like in KK, OLoop has 2 strokes in it
                    strokeTimeSecs /= 2f;
                    yield return HandleCoroutine(DoStroke(strokeTimeSecs, girlIndex));
                    yield return new WaitForSeconds(strokeTimeSecs / 2f);
                }
                yield return HandleCoroutine(DoStroke(strokeTimeSecs, girlIndex));
            }
        }
    }
}
