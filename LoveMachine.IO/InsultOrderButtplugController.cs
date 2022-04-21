using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.IO
{
    internal abstract class InsultOrderButtplugController : ButtplugController
    {
        private static readonly Dictionary<Bone, string> femaleBones = new Dictionary<Bone, string>
        {
            { Bone.Vagina, "HS01_cli" },
            { Bone.LeftHand, "bip01 L Finger1Nub" },
            { Bone.RightHand, "bip01 R Finger1Nub" },
            { Bone.Mouth, "HF01_tongue01" },
            { Bone.LeftBreast, "HS_Breast_LL" },
            { Bone.RightBreast, "HS_Breast_RR" },
            { Bone.LeftFoot, "bip01 L Toe0Nub" },
            { Bone.RightFoot, "bip01 R Toe0Nub" },
        };

        protected override int HeroineCount => 1;

        protected override bool IsHardSex => false;

        protected override bool IsHSceneInterrupted => false;

        protected override float PenisSize => 0.5f;

        protected override int AnimationLayer => 0;

        private GameObject Heroine =>
            GameObject.Find("CH01/CH0001") ?? GameObject.Find("CH02/CH0002");

        protected override Animator GetFemaleAnimator(int _) => Heroine?.GetComponent<Animator>();

        protected override Dictionary<Bone, Transform> GetFemaleBones(int _)
        {
            var heroine = Heroine;
            return femaleBones
                .ToDictionary(kvp => kvp.Key, kvp => FindRecursive(heroine, kvp.Value));
        }

        protected override Transform GetMaleBone() => GameObject.Find("BP00_tamaL").transform;

        protected override string GetPose(int girlIndex)
            => GetFemaleAnimator(girlIndex).GetCurrentAnimatorClipInfo(0)[0].clip.name;

        protected override bool IsIdle(int girlIndex) => GetFemaleAnimator(girlIndex) == null;

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSecondsRealtime(5f);
        }

        private static Transform FindRecursive(GameObject gameObject, string name)
            => gameObject.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == name);
    }

    internal class InsultOrderButtplugVibrationController : InsultOrderButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone)
            => RunVibratorLoop(girlIndex, bone);
    }

    internal class InsultOrderButtplugStrokerController : InsultOrderButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone)
            => RunStrokerLoop(girlIndex, bone);
    }
}
