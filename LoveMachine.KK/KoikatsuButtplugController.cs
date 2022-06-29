using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IllusionUtility.GetUtility;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.KK
{
    internal abstract class KoikatsuButtplugController : ButtplugController
    {
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

        private static readonly List<Bone> aibuBones = new List<Bone>
        {
            Bone.LeftBreast, Bone.RightBreast, Bone.Vagina, Bone.Anus, Bone.LeftButt, Bone.RightButt
        };

        protected HFlag flags;

        protected abstract void HandleFondle(float y, int girlIndex, Bone bone, float timeSecs);

        public void OnStartH(HFlag flags)
        {
            this.flags = flags;
            OnStartH();
        }

        protected override int HeroineCount => flags.lstHeroine.Count;

        protected override bool IsHardSex => flags?.nowAnimStateName?.Contains("SLoop") ?? false;

        protected override int AnimationLayer => 0;

        protected override bool IsHSceneInterrupted => flags.isHSceneEnd;

        protected override float PenisSize => 0.1f;

        protected override bool IsOrgasming(int girlIndex) =>
            orgasmAnimations.Contains(flags.nowAnimStateName);

        protected override Animator GetFemaleAnimator(int girlIndex) =>
            flags.lstHeroine[girlIndex].chaCtrl.animBody;

        protected override Dictionary<Bone, Transform> GetFemaleBones(int girlIndex)
        {
            var bodyBone = flags.lstHeroine[girlIndex].chaCtrl.objBodyBone.transform;
            return femaleBones.ToDictionary(kvp => kvp.Key,
                kvp => bodyBone.FindLoop(kvp.Value).transform);
        }

        protected override Transform GetMaleBone()
        {
            var bodyBone = flags.player.chaCtrl.objBodyBone.transform;
            return bodyBone.FindLoop(MaleBoneName).transform;
        }

        protected override string GetPose(int girlIndex) =>
            // Sideloaded animations all have the same id and name.
            // The only surefire way to uniquely identify an animation seems to be the hash code,
            // since it's based on object reference.
            flags.nowAnimationInfo.GetHashCode()
                + "." + flags.nowAnimationInfo.nameAnimation
                + "." + flags.nowAnimStateName;

        protected override float GetStrokeTimeSecs(int girlIndex, Bone bone)
        {
            float scale = GetAnimatorStateInfo(girlIndex).IsName("OLoop") ? 2 : 1;
            return base.GetStrokeTimeSecs(girlIndex, bone) * scale;
        }

        protected override IEnumerator UntilReady()
        {
            while (flags.lstHeroine.IsNullOrEmpty()
                || flags.lstHeroine.Any(girl => girl.chaCtrl?.animBody == null)
                || flags.player?.chaCtrl?.animBody == null)
            {
                CoreConfig.Logger.LogDebug("Waiting for H Scene to be initialized...");
                yield return new WaitForSeconds(1f);
                if (flags.isHSceneEnd)
                {
                    yield break;
                }
            }
            CoreConfig.Logger.LogDebug("H Scene is now initialized.");
        }

        protected IEnumerator RunAibu(int girlIndex, Bone bone)
        {
            if (!aibuBones.Contains(bone))
            {
                yield break;
            }
            float updateTimeSecs = 0.1f;
            float previousY = 0f;
            while (!flags.isHSceneEnd)
            {
                var y = flags.xy[aibuBones.IndexOf(bone)].y;
                if (previousY != y)
                {
                    HandleFondle(
                        y,
                        girlIndex,
                        bone: bone,
                        timeSecs: updateTimeSecs);
                    previousY = y;
                }
                yield return new WaitForSeconds(updateTimeSecs);
            }
            aibuBones.ForEach(b => DoVibrate(0.0f, girlIndex, bone: b));
        }
    }

    internal class KoikatsuButtplugAnimationController : KoikatsuButtplugController
    {
        protected override void HandleFondle(float y, int girlIndex, Bone bone, float timeSecs) =>
            throw new System.NotImplementedException();

        protected override bool IsIdle(int girlIndex) =>
            throw new System.NotImplementedException();

        protected override IEnumerator Run(int girlIndex, Bone bone)
        {
            var animator = GetFemaleAnimator(girlIndex);
            var playerAnimator = flags.player.chaCtrl.animBody;
            while (!flags.isHSceneEnd)
            {
                var info = animator.GetCurrentAnimatorStateInfo(0);
                if (CoreConfig.ReduceAnimationSpeeds.Value)
                {
                    // nerf the animation speed so the device can keep up with it
                    // OLoop is faster than the rest, about 280ms per stroke at its original speed
                    NerfAnimationSpeeds(info.IsName("OLoop") ? 0.28f : 0.375f,
                        animator, playerAnimator);
                }
                if (CoreConfig.SuppressAnimationBlending.Value)
                {
                    flags.curveMotion = new AnimationCurve(new Keyframe[] { new Keyframe() });
                }
                yield return new WaitForSeconds(.5f);
            }
        }
    }

    internal class KoikatsuButtplugVibrationController : KoikatsuButtplugController
    {
        private static readonly List<HFlag.EMode> houshiModes = new List<HFlag.EMode>
        {
            HFlag.EMode.houshi, HFlag.EMode.houshi3P, HFlag.EMode.houshi3PMMF
        };

        private static readonly List<HFlag.EMode> supportedModes = new List<HFlag.EMode>
        {
            HFlag.EMode.sonyu, HFlag.EMode.sonyu3P, HFlag.EMode.sonyu3PMMF
        }
        .Union(houshiModes).ToList();

        private static readonly List<string> extendedOrgasmAnimations = new List<string>
        {
            "OLoop", "A_OLoop",
            // insertion excitement
            "Pull", "A_Pull", "Insert", "A_Insert"
        }.Union(orgasmAnimations).ToList();

        private static readonly List<string> supportedAnimations = new List<string>
        {
            "WLoop", "SLoop",
            // anal
            "A_WLoop", "A_SLoop"
        }
        .Union(extendedOrgasmAnimations).ToList();

        protected override float VibrationIntensity =>
            IsOrgasming(girlIndex: 0) || houshiModes.Contains(flags.mode) ? 1f : flags.speedCalc;

        protected override bool IsIdle(int girlIndex) =>
            !IsSupportedMode || !IsSupportedAnimation;

        protected override bool IsOrgasming(int girlIndex) =>
            extendedOrgasmAnimations.Contains(flags.nowAnimStateName);

        public bool IsSupportedMode => supportedModes.Contains(flags.mode);

        public bool IsSupportedAnimation => supportedAnimations.Contains(flags.nowAnimStateName);

        protected override IEnumerator Run(int girlIndex, Bone bone) =>
            RunVibratorLoop(girlIndex, bone);

        protected override void HandleFondle(float y, int girlIndex, Bone bone, float timeSecs) =>
            DoVibrate(intensity: y, girlIndex, bone);
    }

    internal class KoikatsuButtplugStrokerController : KoikatsuButtplugController
    {
        private static readonly HFlag.EMode[] supportedModes =
        {
            HFlag.EMode.houshi, HFlag.EMode.sonyu, HFlag.EMode.houshi3P, HFlag.EMode.sonyu3P,
            HFlag.EMode.houshi3PMMF, HFlag.EMode.sonyu3PMMF
        };

        private static readonly List<string> supportedAnimations = new List<string>
        {
            "WLoop", "SLoop",
            // anal
            "A_WLoop", "A_SLoop", "A_OLoop",
            // orgasm
            "OLoop", "A_OLoop",
        }.Union(orgasmAnimations).ToList();

        protected override bool IsIdle(int girlIndex) => !supportedModes.Contains(flags.mode)
                    || !supportedAnimations.Contains(flags.nowAnimStateName)
                    || flags.speed < 1;

        protected override void HandleFondle(float y, int girlIndex, Bone bone, float timeSecs) =>
            MoveStroker(position: y, timeSecs, girlIndex, bone);

        protected override IEnumerator Run(int girlIndex, Bone bone) =>
            RunStrokerLoop(girlIndex, bone);
    }

    internal class KoikatsuButtplugRotatorController : KoikatsuButtplugStrokerController
    {
        protected override void HandleFondle(float y, int girlIndex, Bone bone, float timeSecs) =>
            throw new System.NotImplementedException();

        protected override IEnumerator Run(int girlIndex, Bone bone) =>
            RunRotatorLoop(girlIndex, bone);
    }

    internal class KoikatsuButtplugAibuVibrationController : KoikatsuButtplugVibrationController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone) =>
            RunAibu(girlIndex, bone);
    }

    internal class KoikatsuButtplugAibuStrokerController : KoikatsuButtplugStrokerController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone) =>
            RunAibu(girlIndex, bone);
    }
}
