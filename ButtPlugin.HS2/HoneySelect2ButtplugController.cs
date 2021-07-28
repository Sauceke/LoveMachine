using ButtPlugin.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ButtPlugin.HS2
{
    public abstract class HoneySelect2ButtplugController : ButtplugController
    {
        protected override int HeroineCount => Array.FindAll(hScene.GetFemales(), f => f != null).Length;

        protected override int AnimationLayer => throw new NotImplementedException();

        protected override string CurrentAnimationState => throw new NotImplementedException();

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

        protected override Animator GetFemaleAnimator(int girlIndex)
        {
            throw new NotImplementedException();
        }

        protected override Animator GetMaleAnimator()
        {
            throw new NotImplementedException();
        }

        protected override List<Transform> GetFemaleBones(int girlIndex)
        {
            throw new NotImplementedException();
        }

        protected override List<Transform> GetMaleBones()
        {
            throw new NotImplementedException();
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
