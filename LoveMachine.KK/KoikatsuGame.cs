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

        private static readonly List<HFlag.EMode> houshiModes = new List<HFlag.EMode>
        {
            HFlag.EMode.houshi, HFlag.EMode.houshi3P, HFlag.EMode.houshi3PMMF
        };

        private const string MaleBoneName = "k_f_tamaL_00"; // left testicle

        private static readonly Dictionary<Bone, string> femaleBones = new Dictionary<Bone, string>
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

        protected static readonly List<string> orgasmAnimations = new List<string>
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

        protected override int HeroineCount => Flags.lstHeroine.Count;

        protected override bool IsHardSex => Flags?.nowAnimStateName?.Contains("SLoop") ?? false;

        public override int AnimationLayer => 0;

        protected override bool IsHSceneInterrupted => Flags.isHSceneEnd;

        protected override float PenisSize => 0.1f;

        protected override float VibrationIntensity =>
            IsOrgasming(girlIndex: 0) || houshiModes.Contains(Flags.mode) ? 1f : Flags.speedCalc;

        protected override bool IsIdle(int girlIndex) => !supportedModes.Contains(Flags.mode)
                    || !supportedAnimations.Contains(Flags.nowAnimStateName)
                    || Flags.speed < 1;

        protected override bool IsOrgasming(int girlIndex) =>
            orgasmAnimations.Contains(Flags.nowAnimStateName);

        public override Animator GetFemaleAnimator(int girlIndex) =>
            Flags.lstHeroine[girlIndex].chaCtrl.animBody;

        protected override Dictionary<Bone, Transform> GetFemaleBones(int girlIndex)
        {
            var bodyBone = Flags.lstHeroine[girlIndex].chaCtrl.objBodyBone.transform;
            return femaleBones.ToDictionary(kvp => kvp.Key,
                kvp => bodyBone.FindLoop(kvp.Value).transform);
        }

        protected override Transform GetMaleBone()
        {
            var bodyBone = Flags.player.chaCtrl.objBodyBone.transform;
            return bodyBone.FindLoop(MaleBoneName).transform;
        }

        protected override string GetPose(int girlIndex) =>
            // Sideloaded animations all have the same id and name.
            // The only surefire way to uniquely identify an animation seems to be the hash code,
            // since it's based on object reference.
            Flags.nowAnimationInfo.GetHashCode()
                + "." + Flags.nowAnimationInfo.nameAnimation
                + "." + Flags.nowAnimStateName;

        protected override void GetAnimState(int girlIndex, out float normalizedTime,
            out float length, out float speed)
        {
            base.GetAnimState(girlIndex, out normalizedTime, out float baseLength, out speed);
            length = GetAnimatorStateInfo(girlIndex).IsName("OLoop") ? baseLength * 2f : baseLength;
        }

        protected override IEnumerator UntilReady()
        {
            while (Flags.lstHeroine.IsNullOrEmpty()
                || Flags.lstHeroine.Any(girl => girl.chaCtrl?.animBody == null)
                || Flags.player?.chaCtrl?.animBody == null)
            {
                CoreConfig.Logger.LogDebug("Waiting for H Scene to be initialized...");
                yield return new WaitForSeconds(1f);
                if (Flags.isHSceneEnd)
                {
                    yield break;
                }
            }
            CoreConfig.Logger.LogDebug("H Scene is now initialized.");
        }
    }
}
