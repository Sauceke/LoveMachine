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

        protected override int HeroineCount => Array.FindAll(hScene.GetFemales(), f => f != null).Length;

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
                || hScene.GetFemales()[0] == null)
            {
                yield return new WaitForSeconds(.1f);
            }
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
            while (true)
            {
                AnimatorStateInfo info() => hScene.GetFemales()[girlIndex].getAnimatorStateInfo(0);
                yield return HandleCoroutine(WaitForUpStroke(info, girlIndex));
                yield return HandleCoroutine(DoStroke(GetStrokeTimeSecs(info()), girlIndex));
            }
        }
    }
}
