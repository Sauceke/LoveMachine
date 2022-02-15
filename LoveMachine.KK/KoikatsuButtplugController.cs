using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IllusionUtility.GetUtility;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.KK
{
    public abstract class KoikatsuButtplugController : ButtplugController
    {
        private const string MaleBoneName = "k_f_tamaL_00"; // left testicle

        private static readonly Dictionary<Bone, string> femaleBones
            = new Dictionary<Bone, string>
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

        protected override Animator GetFemaleAnimator(int girlIndex)
            => flags.lstHeroine[girlIndex].chaCtrl.animBody;

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

        protected override string GetPose(int girlIndex)
        {
            // Sideloaded animations all have the same id and name.
            // The only surefire way to uniquely identify an animation seems to be the hash code,
            // since it's based on object reference.
            return flags.nowAnimationInfo.GetHashCode()
                + "." + flags.nowAnimationInfo.nameAnimation
                + "." + flags.nowAnimStateName;
        }

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

    public class KoikatsuButtplugAnimationController : KoikatsuButtplugController
    {
        protected override void HandleFondle(float y, int girlIndex, Bone bone, float timeSecs)
            => throw new System.NotImplementedException();

        protected override bool IsIdle(int girlIndex)
            => throw new System.NotImplementedException();

        protected override IEnumerator Run(int girlIndex, Bone bone)
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

        protected override IEnumerator Run(int girlIndex, Bone bone)
            => RunVibratorLoop(girlIndex, bone);

        protected override void HandleFondle(float y, int girlIndex, Bone bone, float timeSecs)
            => DoVibrate(intensity: y, girlIndex, bone);
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

        protected override void HandleFondle(float y, int girlIndex, Bone bone, float timeSecs)
            => MoveStroker(position: y, timeSecs, girlIndex, bone);

        protected override IEnumerator Run(int girlIndex, Bone bone)
             => RunStrokerLoop(girlIndex, bone);
    }

    public class KoikatsuButtplugAibuVibrationController : KoikatsuButtplugVibrationController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone)
            => RunAibu(girlIndex, bone);
    }

    public class KoikatsuButtplugAibuStrokerController : KoikatsuButtplugStrokerController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone)
             => RunAibu(girlIndex, bone);
    }

    public class KoikatsuButtplugAibuDepthController : KoikatsuButtplugController
    {
        private CalorDepthPOC calor;

        protected override void HandleFondle(float y, int girlIndex, Bone bone, float timeSecs)
            => throw new System.NotImplementedException();

        protected override bool IsIdle(int girlIndex)
            => throw new System.NotImplementedException();

        private void Init()
        {
            calor = gameObject.GetComponent<CalorDepthPOC>();
        }

        protected override IEnumerator Run(int girlIndex, Bone bone)
        {
            if (girlIndex != 0 || bone != Bone.Auto)
            {
                yield break;
            }
            Init();
            while (true)
            {
                yield return new WaitForEndOfFrame();
                if (!TryGetWaveInfo(0, Bone.Auto, out var result))
                {
                    GetFemaleAnimator(0).speed = 1;
                    flags.player.chaCtrl.animBody.speed = 1;
                    continue;
                }
                if (!calor.TryGetNewDepth(peek: false, out float depth))
                {
                    continue;
                }
                if (depth < 0 && flags.nowAnimStateName != "Idle")
                {
                    flags.click = HFlag.ClickKind.pull;
                    continue;
                }
                if (flags.nowAnimStateName == "Idle")
                {
                    flags.click = HFlag.ClickKind.insert;
                    yield return new WaitForSeconds(5f);
                    flags.click = HFlag.ClickKind.modeChange;
                    flags.speedCalc = 0.5f;
                    yield return new WaitForSeconds(2f);
                }
                GetFemaleAnimator(0).speed = 0;
                flags.player.chaCtrl.animBody.speed = 0;
                float targetNormTime = (result.Phase + 1.5f - depth / 2f) % 1f;
                float startNormTime = GetFemaleAnimator(0).GetCurrentAnimatorStateInfo(AnimationLayer)
                    .normalizedTime % 1f;
                int steps = 20;
                float step = (targetNormTime - startNormTime) / steps;
                for (int i = 1; i <= steps; i++)
                {
                    if (calor.TryGetNewDepth(peek: true, out _))
                    {
                        break;
                    }
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
