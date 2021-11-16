using System.Collections;
using System.Collections.Generic;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.AGH
{
    public abstract class HoukagoRinkanButtplugController : ButtplugController
    {
        protected override int HeroineCount => 1;

        protected override bool IsHardSex => true;

        protected override int AnimationLayer => 0;

        protected override int CurrentAnimationStateHash
            => GetFemaleAnimator(0).GetCurrentAnimatorStateInfo(0).fullPathHash;

        protected override bool IsHSceneInterrupted => false;

        protected override Animator GetFemaleAnimator(int girlIndex)
            => GameObject.Find("CH01/CH0001")?.GetComponent<Animator>()
            ?? GameObject.Find("CH02/CH0002").GetComponent<Animator>();

        protected override List<Transform> GetFemaleBones(int girlIndex)
        {
            var chara = GameObject.Find("CH01") != null ? "CH01/CH0001/" : "CH02/CH0002/";
            return new List<Transform>
            {
                // there are CH01 prefixed bones under CH02, wtf
                GameObject.Find(chara + "CH01_pussy00").transform,
                GameObject.Find(chara + "CH01_mouthIN").transform,
                GameObject.Find(chara + "CH01_nipple").transform
            };
        }
        
        protected override Animator GetMaleAnimator() => null;

        protected override Transform GetMaleBone()
            => GameObject.Find("PC01/PC/HS_kiten_PC/PC00Bip/PC00Bip Pelvis").transform;

        protected override string GetPose(int girlIndex)
            => GetFemaleAnimator(girlIndex).GetCurrentAnimatorStateInfo(0).fullPathHash + "."
                + girlIndex;

        protected override int GetStrokesPerAnimationCycle(int girlIndex) => 1;

        protected override bool IsIdle(int girlIndex) => false;

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSecondsRealtime(5f);
        }
    }

    public class HoukagoRinkanButtplugVibrationController : HoukagoRinkanButtplugController
    {
        protected override IEnumerator Run(int girlIndex, int boneIndex)
            => RunVibratorLoop(girlIndex, boneIndex);
    }

    public class HoukagoRinkanButtplugStrokerController : HoukagoRinkanButtplugController
    {
        protected override IEnumerator Run(int girlIndex, int boneIndex)
            => RunStrokerLoop(girlIndex, boneIndex);
    }
}
