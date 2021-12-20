using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.AGH
{
    public abstract class HoukagoRinkanButtplugController : ButtplugController
    {
        private static readonly List<string> SayaBones = new List<string>
        {
            "CH01/CH0001/CH01_pussy00",
            "bip01 L Finger1Nub",
            "BF01_tongue01",
            "HS_Breast_LL"
        };

        private static readonly List<string> ElenaBones = new List<string>
        {
            "CH02/CH0002/CH01_pussy00",
            "bip01 L Finger1Nub_02",
            "BF01_tongue01_02",
            "HS_Breast_LL_02"
        };

        protected override int HeroineCount => 1;

        protected override bool IsHardSex => true;

        protected override int AnimationLayer => 0;

        protected override bool IsHSceneInterrupted => false;

        protected override Animator GetFemaleAnimator(int girlIndex)
            => (GameObject.Find("CH01/CH0001") ?? GameObject.Find("CH02/CH0002"))
                .GetComponent<Animator>();

        protected override List<Transform> GetFemaleBones(int girlIndex)
            => (GameObject.Find("CH01") != null ? SayaBones : ElenaBones)
                .Select(bone => GameObject.Find(bone).transform).ToList();

        protected override Transform GetMaleBone()
            => GameObject.Find("PC01/PC/HS_kiten_PC/PC00Bip/PC00Bip Pelvis").transform;

        protected override string GetPose(int girlIndex)
            => GetFemaleAnimator(girlIndex).GetCurrentAnimatorClipInfo(0)[0].clip.name + "."
                + girlIndex;

        protected override bool IsIdle(int girlIndex) => GetFemaleAnimator(girlIndex) == null;

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
