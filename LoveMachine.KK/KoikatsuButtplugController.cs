using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Bootstrap;
using IllusionUtility.GetUtility;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.KK
{
    public abstract class KoikatsuButtplugController : ButtplugController
    {
        private const string MaleBoneName = "k_f_tamaL_00"; // left testicle

        internal static readonly Dictionary<string, string> femaleBones
            = new Dictionary<string, string>
        {
            { "k_f_munenipL_00", "Left Breast"},
            { "k_f_munenipR_00", "Right Breast"},
            { "cf_n_pee", "Pussy"},
            { "k_f_ana_00", "Anus"},
            { "k_f_siriL_00", "Left Butt"},
            { "k_f_siriR_00", "Right Butt"},
            { "cf_J_MouthCavity", "Mouth"},
            { "cf_j_index04_L", "Left Hand"},
            { "cf_j_index04_R", "Right Hand"},
            { "k_f_toeL_00", "Left Foot"},
            { "k_f_toeR_00", "Right Foot"},
        };

        protected HFlag flags;

        protected abstract void HandleFondle(float y, int girlIndex, int boneIndex,
            float timeSecs);

        public void OnStartH(HFlag flags)
        {
            this.flags = flags;
            OnStartH();
        }

        protected override int HeroineCount => flags.lstHeroine.Count;

        protected override bool IsHardSex => flags?.nowAnimStateName?.Contains("SLoop") ?? false;

        protected override int AnimationLayer => 0;

        protected override bool IsHSceneInterrupted => flags.isHSceneEnd;

        protected override Animator GetFemaleAnimator(int girlIndex)
            => flags.lstHeroine[girlIndex].chaCtrl.animBody;

        protected override List<Transform> GetFemaleBones(int girlIndex)
        {
            var bodyBone = flags.lstHeroine[girlIndex].chaCtrl.objBodyBone.transform;
            return femaleBones.Keys.Select(name => bodyBone.FindLoop(name).transform).ToList();
        }

        protected override Transform GetMaleBone()
        {
            var bodyBone = flags.player.chaCtrl.objBodyBone.transform;
            return bodyBone.FindLoop(MaleBoneName).transform;
        }

        protected override string GetPose(int girlIndex)
        {
            // Sideloaded animations all have the same id and name.
            // The only surefire way to uniquely identify an animation seems to be the hash code,
            // since it's based on object reference.
            return flags.nowAnimationInfo.GetHashCode()
                + "." + flags.nowAnimationInfo.nameAnimation
                + "." + flags.nowAnimStateName
                + "." + girlIndex;
        }

        protected override float GetStrokeTimeSecs(int girlIndex, int boneIndex)
        {
            float scale = GetAnimatorStateInfo(girlIndex).IsName("OLoop") ? 2 : 1;
            return base.GetStrokeTimeSecs(girlIndex, boneIndex) * scale;
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

        protected IEnumerator RunAibu(int girlIndex, int boneIndex)
        {
            if (boneIndex == 0 || boneIndex > flags.xy.Length)
            {
                yield break;
            }
            float updateTimeSecs = 0.1f;
            float previousY = 0f;
            while (!flags.isHSceneEnd)
            {
                var y = flags.xy[boneIndex - 1].y;
                if (previousY != y)
                {
                    HandleFondle(
                        y,
                        girlIndex,
                        boneIndex: boneIndex,
                        timeSecs: updateTimeSecs);
                    previousY = y;
                }
                yield return new WaitForSeconds(updateTimeSecs);
            }
            for (int i = 0; i < 6; i++)
            {
                DoVibrate(0.0f, girlIndex, boneIndex: i);
            }
        }
    }

    public class KoikatsuButtplugAnimationController : KoikatsuButtplugController
    {
        protected override void HandleFondle(float y, int girlIndex, int boneIndex,
            float timeSecs)
        { }

        protected override bool IsIdle(int girlIndex)
            => throw new System.NotImplementedException();

        protected override IEnumerator Run(int girlIndex, int boneIndex)
        {
            var animator = GetFemaleAnimator(girlIndex);
            var playerAnimator = flags.player.chaCtrl.animBody;
            while (!flags.isHSceneEnd)
            {
                var info = animator.GetCurrentAnimatorStateInfo(0);
                if (KKLoveMachine.ReduceAnimationSpeeds.Value)
                {
                    // nerf the animation speed so the device can keep up with it
                    // OLoop is faster than the rest, about 280ms per stroke at its original speed
                    NerfAnimationSpeeds(info.IsName("OLoop") ? 0.28f : 0.375f,
                        animator, playerAnimator);
                }
                if (KKLoveMachine.SuppressAnimationBlending.Value)
                {
                    flags.curveMotion = new AnimationCurve(new Keyframe[] { new Keyframe() });
                }
                yield return new WaitForSeconds(.5f);
            }
        }
    }

    public class KoikatsuButtplugVibrationController : KoikatsuButtplugController
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

        private static readonly List<string> orgasmAnimations = new List<string>
        {
            "OLoop", "A_OLoop",

            // ejaculation
            "OUT_START", "OUT_LOOP", "IN_START", "IN_LOOP",
            "M_OUT_Start", "M_OUT_Loop", "M_IN_Start", "M_IN_Loop",
            "WS_IN_Start", "WS_IN_Loop", "SS_IN_Start", "SS_IN_Loop",
            "A_WS_IN_Start", "A_WS_IN_Loop", "A_SS_IN_Start", "A_SS_IN_Loop",

            // insertion excitement
            "Pull", "A_Pull", "Insert", "A_Insert"
        };

        private static readonly List<string> supportedAnimations = new List<string>
        {
            "WLoop", "SLoop",
            // anal
            "A_WLoop", "A_SLoop", "A_OLoop"
        }
        .Union(orgasmAnimations).ToList();

        protected override float VibrationIntensity
        {
            get
            {
                if (IsOrgasm || houshiModes.Contains(flags.mode))
                {
                    return 1f;
                }
                return flags.speedCalc;
            }
        }

        protected override bool IsIdle(int girlIndex)
            => !IsSupportedMode || !IsSupportedAnimation;

        public bool IsOrgasm
        {
            get { return orgasmAnimations.Contains(flags.nowAnimStateName); }
        }

        public bool IsSupportedMode
        {
            get { return supportedModes.Contains(flags.mode); }
        }

        public bool IsSupportedAnimation
        {
            get { return supportedAnimations.Contains(flags.nowAnimStateName); }
        }

        protected override IEnumerator Run(int girlIndex, int boneIndex)
            => RunVibratorLoop(girlIndex, boneIndex);

        protected override void HandleFondle(float y, int girlIndex, int boneIndex, float timeSecs)
            => DoVibrate(intensity: y, girlIndex, boneIndex: boneIndex);
    }

    public class KoikatsuButtplugStrokerController : KoikatsuButtplugController
    {
        private static readonly List<HFlag.EMode> supportedModes = new List<HFlag.EMode>
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
        };

        protected override bool IsIdle(int girlIndex) => !supportedModes.Contains(flags.mode)
                    || !supportedAnimations.Contains(flags.nowAnimStateName)
                    || flags.speed < 1;

        protected override void HandleFondle(float y, int girlIndex, int boneIndex, float timeSecs)
            => MoveStroker(position: y, timeSecs, girlIndex, boneIndex);

        protected override IEnumerator Run(int girlIndex, int boneIndex)
             => RunStrokerLoop(girlIndex, boneIndex);
    }

    public class KoikatsuButtplugAibuVibrationController : KoikatsuButtplugVibrationController
    {
        protected override IEnumerator Run(int girlIndex, int boneIndex)
            => RunAibu(girlIndex, boneIndex);
    }

    public class KoikatsuButtplugAibuStrokerController : KoikatsuButtplugStrokerController
    {
        protected override IEnumerator Run(int girlIndex, int boneIndex)
             => RunAibu(girlIndex, boneIndex);
    }

    public class KoikatsuButtplugAibuDepthController : KoikatsuButtplugController
    {
        protected override void HandleFondle(float y, int girlIndex, int boneIndex, float timeSecs)
        {
            throw new System.NotImplementedException();
        }

        protected override bool IsIdle(int girlIndex)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerator Run(int girlIndex, int boneIndex)
        {
            if (girlIndex != 0 || boneIndex != 0)
            {
                yield break;
            }
            var calor = Chainloader.ManagerObject.GetComponent<CalorDepthPOC>();
            while (true)
            {
                yield return new WaitForEndOfFrame();
                if (!TryGetPhase(0, 0, out float phase))
                {
                    GetFemaleAnimator(0).speed = 1;
                    flags.player.chaCtrl.animBody.speed = 1;
                    continue;
                }
                if (!calor.TryGetNewDepth(out float depth))
                {
                    continue;
                }
                GetFemaleAnimator(0).speed = 0;
                flags.player.chaCtrl.animBody.speed = 0;
                float targetNormTime = (phase + 1.5f - depth / 2f) % 1f;
                float startNormTime = GetFemaleAnimator(0).GetCurrentAnimatorStateInfo(AnimationLayer)
                    .normalizedTime % 1f;
                int steps = 20;
                float step = (targetNormTime - startNormTime) / steps;
                for (int i = 1; i <= steps; i++)
                {
                    float normTime = startNormTime + step * i;
                    var animStateHash = GetAnimatorStateInfo(0).fullPathHash;
                    GetFemaleAnimator(0).Play(animStateHash, AnimationLayer, normTime);
                    flags.player.chaCtrl.animBody.Play(animStateHash, AnimationLayer, normTime);
                    yield return new WaitForEndOfFrame();
                }
            }
        }

    }
}
