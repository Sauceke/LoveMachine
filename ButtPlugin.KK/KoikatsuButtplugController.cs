using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ButtPlugin.Core;
using IllusionUtility.GetUtility;
using UnityEngine;

namespace ButtPlugin.KK
{
    public abstract class KoikatsuButtplugController : ButtplugController
    {
        private readonly List<string> femaleBoneNames = new List<string>
        {
            "cf_n_pee", // pussy
            "cf_j_index04_R", "cf_j_index04_L", // index fingers
            "cf_J_MouthCavity",
            "k_f_munenipR_00" // right nipple
        };
        private readonly List<string> maleBoneNames = new List<string>
        {
            "k_f_tamaL_00" // left testicle
        };

        protected HFlag flags;

        public void OnStartH(HFlag flags)
        {
            this.flags = flags;
            OnStartH();
        }

        protected override int HeroineCount => flags.lstHeroine.Count;

        protected override int AnimationLayer => 0;

        protected override int CurrentAnimationStateHash
            => Animator.StringToHash(flags.nowAnimStateName);

        protected override Animator GetFemaleAnimator(int girlIndex)
            => flags.lstHeroine[girlIndex].chaCtrl.animBody;

        protected override Animator GetMaleAnimator() => flags.player.chaCtrl.animBody;

        protected override List<Transform> GetFemaleBones(int girlIndex)
        {
            var bodyBone = flags.lstHeroine[girlIndex].chaCtrl.objBodyBone.transform;
            return femaleBoneNames.Select(name => bodyBone.FindLoop(name).transform).ToList();
        }

        protected override List<Transform> GetMaleBones()
        {
            var bodyBone = flags.player.chaCtrl.objBodyBone.transform;
            return maleBoneNames.Select(name => bodyBone.FindLoop(name).transform).ToList();
        }

        protected override string GetPose(int girlIndex)
        {
            return flags.nowAnimationInfo.mode
                + "." + flags.nowAnimationInfo.id
                + "." + flags.nowAnimStateName
                + "." + girlIndex;
        }

        protected override IEnumerator UntilReady()
        {
            while (flags.lstHeroine.IsNullOrEmpty()
                || flags.lstHeroine.Any(girl => girl.chaCtrl?.animBody == null)
                || flags.player?.chaCtrl?.animBody == null)
            {
                yield return new WaitForSeconds(1f);
                if (flags.isHSceneEnd)
                {
                    yield break;
                }
            }
        }
    }

    public class KoikatsuButtplugVibrationController : KoikatsuButtplugController
    {
        private static readonly List<HFlag.EMode> supportedMaleModes = new List<HFlag.EMode>
        {
            HFlag.EMode.houshi, HFlag.EMode.sonyu, HFlag.EMode.houshi3P, HFlag.EMode.sonyu3P,
        };

        private static readonly List<HFlag.EMode> supportedFemaleModes = new List<HFlag.EMode>
        {
            HFlag.EMode.sonyu, HFlag.EMode.sonyu3P,
            HFlag.EMode.masturbation, HFlag.EMode.lesbian
        };

        private IEnumerable<HFlag.EMode> supportedModes
        {
            get
            {
                IEnumerable<HFlag.EMode> modes = new List<HFlag.EMode>();
                if (IsFemale)
                {
                    modes = modes.Concat(supportedFemaleModes);
                }
                if (IsMale)
                {
                    modes = modes.Concat(supportedMaleModes);
                }
                return modes;
            }
        }

        private static readonly List<string> supportedAnimations = new List<string>
        {
            "WLoop", "SLoop",
            // masturbation
            "MLoop", 
            // anal
            "A_WLoop", "A_SLoop", "A_OLoop",

            // orgasm
            "OLoop", "A_OLoop",

            // ejaculation
            "OUT_START", "OUT_LOOP", "IN_START", "IN_LOOP",
            "M_OUT_Start", "M_OUT_Loop", "M_IN_Start", "M_IN_Loop",
            "WS_IN_Start", "WS_IN_Loop", "SS_IN_Start", "SS_IN_Loop",
            "A_WS_IN_Start", "A_WS_IN_Loop", "A_SS_IN_Start", "A_SS_IN_Loop",
            
            // insertion
            "Pull", "A_Pull", "Insert", "A_Insert"
        };

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

        protected override IEnumerator Run(int girlIndex)
        {
            while (!flags.isHSceneEnd)
            {
                if (CoreConfig.EnableVibrate.Value == VibrationMode.Off)
                {
                    yield return new WaitForSeconds(1.0f);
                    continue;
                }

                if (!IsSupportedMode || !IsSupportedAnimation)
                {
                    // stops vibration when not being lewd
                    DoVibrate(0.0f, girlIndex);
                    yield return new WaitForSecondsRealtime(1.0f / CoreConfig.VibrationUpdateFrequency.Value);
                    continue;
                }

                var animator = flags.lstHeroine[girlIndex].chaCtrl.animBody;

                var speed = flags.speedCalc;
                var info = animator.GetCurrentAnimatorStateInfo(0);
                var minVibration = 0.2f;

                // service mode goes into OLoop once male excitement exceeds its threshold
                if (IsOrgasm)
                {
                    speed = 1.0f;
                    minVibration = 0.6f;
                }

                if (IsFemale && flags.mode == HFlag.EMode.masturbation
                    && CoreConfig.SyncVibrationWithAnimation.Value)
                {
                    // masturbation is on a non-speed controlled animation
                    // it has a fixed order of the animation loops, so we can apply a base strength
                    //   relative to the intensity of the animation
                    switch (flags.nowAnimStateName)
                    {
                        case "WLoop":
                            speed = 0.4f;
                            minVibration = 0.2f;
                            break;
                        case "MLoop":
                            speed = 0.8f;
                            minVibration = 0.4f;
                            break;
                        case "SLoop":
                            speed = 1.0f;
                            minVibration = 0.4f;
                            break;
                    }
                }
                yield return HandleCoroutine(VibrateWithAnimation(info, girlIndex, speed, minVibration));
            }
            // turn off vibration since there's nothing to animate against
            // this state can happen if H is ended while the animation is not in Idle
            DoVibrate(0.0f, girlIndex);
        }
    }

    public class KoikatsuButtplugStrokerController : KoikatsuButtplugController
    {
        private static readonly List<HFlag.EMode> supportedModes = new List<HFlag.EMode>
        {
            HFlag.EMode.houshi, HFlag.EMode.sonyu, HFlag.EMode.houshi3P, HFlag.EMode.sonyu3P
        };

        private static readonly List<string> supportedAnimations = new List<string>
        {
            "WLoop", "SLoop",
            // anal
            "A_WLoop", "A_SLoop", "A_OLoop",
            // orgasm
            "OLoop", "A_OLoop",
        };

        protected override IEnumerator Run(int girlIndex)
        {
            var animator = flags.lstHeroine[girlIndex].chaCtrl.animBody;
            var playerAnimator = flags.player.chaCtrl.animBody;
            while (!flags.isHSceneEnd)
            {
                if (!supportedModes.Contains(flags.mode)
                    || !supportedAnimations.Contains(flags.nowAnimStateName)
                    || flags.speed < 1)
                {
                    yield return new WaitForSeconds(.1f);
                    continue;
                }
                AnimatorStateInfo info() => animator.GetCurrentAnimatorStateInfo(0);
                // nerf the animation speed so the device can keep up with it
                // OLoop is faster than the rest, about 280ms per stroke at its original speed
                NerfAnimationSpeeds(
                    info().IsName("OLoop") ? 0.28f : 0.375f, animator, playerAnimator);
                yield return HandleCoroutine(WaitForUpStroke(info, girlIndex));
                float strokeTimeSecs = GetStrokeTimeSecs(info());
                if (info().IsName("OLoop"))
                {
                    // no idea what's the deal with OLoop
                    // it seems to loop after two strokes
                    yield return HandleCoroutine(DoStroke(strokeTimeSecs, girlIndex));
                    yield return new WaitForSeconds(strokeTimeSecs / 2f);
                }
                yield return HandleCoroutine(DoStroke(strokeTimeSecs, girlIndex));
            }
        }
    }
}
