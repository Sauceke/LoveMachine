using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IllusionUtility.GetUtility;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.KK
{
    internal sealed class KoikatsuGame : GameDescriptor
    {
        private static readonly HFlag.EMode[] supportedModes =
        {
            HFlag.EMode.houshi, HFlag.EMode.sonyu, HFlag.EMode.houshi3P, HFlag.EMode.sonyu3P,
            HFlag.EMode.houshi3PMMF, HFlag.EMode.sonyu3PMMF
        };

        private static readonly List<string> orgasmAnimations = new List<string>
        {
            "OUT_START", "OUT_LOOP", "IN_START", "IN_LOOP", "IN_Start", "IN_Loop",
            "M_OUT_Start", "M_OUT_Loop", "M_IN_Start", "M_IN_Loop",
            "WS_IN_Start", "WS_IN_Loop", "SS_IN_Start", "SS_IN_Loop",
            "A_WS_IN_Start", "A_WS_IN_Loop", "A_SS_IN_Start", "A_SS_IN_Loop",
        };

        private static readonly List<string> supportedAnimations = new List<string>
        {
            "WLoop", "SLoop",
            // anal
            "A_WLoop", "A_SLoop", "A_OLoop",
            // orgasm
            "OLoop", "A_OLoop",
        }.Union(orgasmAnimations).ToList();

        public HFlag Flags { get; private set; }

        public void OnStartH(HFlag flags)
        {
            Flags = flags;
            StartH();
        }

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.LeftBreast, "k_f_munenipL_00" },
            { Bone.RightBreast, "k_f_munenipR_00" },
            { Bone.Vagina, "cf_n_pee" },
            { Bone.Anus, "k_f_ana_00" },
            { Bone.LeftButt, "k_f_siriL_00" },
            { Bone.RightButt, "k_f_siriR_00" },
            { Bone.Mouth, "cf_J_MouthCavity" },
            { Bone.LeftHand, "cf_j_index04_L" },
            { Bone.RightHand, "cf_j_index04_R" },
            { Bone.LeftFoot, "k_f_toeL_00" },
            { Bone.RightFoot, "k_f_toeR_00" },
        };

        protected override int HeroineCount => Flags.lstHeroine.Count;

        protected override int MaxHeroineCount => 2;

        protected override bool IsHardSex => Flags?.nowAnimStateName?.Contains("SLoop") ?? false;

        public override int AnimationLayer => 0;

        protected override bool IsHSceneInterrupted => Flags.isHSceneEnd;

        protected override float PenisSize => 0.1f;

        protected override float VibrationIntensity => Flags.speedCalc == 0f ? 1f : Flags.speedCalc;

        protected override bool IsIdle(int girlIndex) => !supportedModes.Contains(Flags.mode)
                    || !supportedAnimations.Contains(Flags.nowAnimStateName)
                    || Flags.speed < 1f;

        protected override bool IsOrgasming(int girlIndex) =>
            orgasmAnimations.Contains(Flags.nowAnimStateName);

        public override Animator GetFemaleAnimator(int girlIndex) =>
            Flags.lstHeroine[girlIndex].chaCtrl.animBody;

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            Flags.lstHeroine[girlIndex].chaCtrl.objBodyBone;

        protected override Transform GetDickBase() =>
            Flags.player.chaCtrl.objBodyBone.transform.FindLoop("k_f_tamaL_00").transform;

        protected override string GetPose(int girlIndex) =>
            // Sideloaded animations all have the same id and name.
            // The only surefire way to uniquely identify an animation seems
            // to be the hash code, since it's based on object reference.
            Flags.nowAnimationInfo.GetHashCode()
                + "." + Flags.nowAnimationInfo.nameAnimation
                + "." + Flags.nowAnimStateName;

        protected override IEnumerator WaitAfterPoseChange()
        {
            // play nicely with CrossFader
            while (!GetAnimatorStateInfo(0).IsName(Flags.nowAnimStateName))
            {
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.1f);
        }

        protected override IEnumerator UntilReady()
        {
            yield return new WaitWhile(() => !Flags.isHSceneEnd
                && (Flags.lstHeroine.IsNullOrEmpty()
                || Flags.lstHeroine.Any(girl => girl.chaCtrl?.animBody == null)
                || Flags.player?.chaCtrl?.animBody == null));
        }
    }
}
