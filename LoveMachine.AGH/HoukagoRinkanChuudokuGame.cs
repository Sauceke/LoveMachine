using System.Collections;
using System.Collections.Generic;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.AGH
{
    internal sealed class HoukagoRinkanChuudokuGame : GameDescriptor
    {
        private static readonly Dictionary<Bone, string> sayaBones = new Dictionary<Bone, string>
        {
            { Bone.Vagina, "HS01_cli" },
            { Bone.LeftHand, "bip01 L Finger1Nub" },
            { Bone.Mouth, "BF01_tongue01" },
            { Bone.LeftBreast, "HS_Breast_LL" },
        };

        private static readonly Dictionary<Bone, string> elenaBones = new Dictionary<Bone, string>
        {
            { Bone.Vagina, "HS01_cli_02" },
            { Bone.LeftHand, "bip01 L Finger1Nub_02" },
            { Bone.Mouth, "BF01_tongue01_02" },
            { Bone.LeftBreast, "HS_Breast_LL_02" },
        };

        private Animator femaleAnimator;
        private Transform coom;

        protected override Dictionary<Bone, string> FemaleBoneNames =>
            GameObject.Find("CH01") != null ? sayaBones : elenaBones;

        protected override int HeroineCount => 1;

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => true;

        public override int AnimationLayer => 0;

        protected override bool IsHSceneInterrupted => false;

        protected override float PenisSize => 0.5f;

        protected override float MinOrgasmDurationSecs => 6f;

        public override Animator GetFemaleAnimator(int girlIndex) => femaleAnimator;

        protected override GameObject GetFemaleRoot(int _girlIndex) => null;

        protected override Transform GetDickBase() => GameObject.Find("BP00_tamaL").transform;

        protected override string GetPose(int girlIndex) =>
            femaleAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        protected override bool IsIdle(int girlIndex) => femaleAnimator == null;

        protected override bool IsOrgasming(int girlIndex) => coom.localPosition != Vector3.zero;

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSeconds(5f);
            femaleAnimator = (GameObject.Find("CH01/CH0001") ?? GameObject.Find("CH02/CH0002"))
                .GetComponent<Animator>();
            coom = GameObject.Find("PC01/PC/HS01_SE04").transform;
        }
    }
}
