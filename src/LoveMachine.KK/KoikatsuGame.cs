using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using HarmonyLib;
using IllusionUtility.GetUtility;
using LoveMachine.Core.Common;
using UnityEngine;

namespace LoveMachine.KK
{
    internal sealed class KoikatsuGame : AbstractKoikatsuGame
    {
        private static readonly HFlag.EMode[] playerlessModes =
            { HFlag.EMode.lesbian, HFlag.EMode.masturbation };
        
        private static readonly HFlag.EMode[] supportedModes = new []
        {
            HFlag.EMode.houshi, HFlag.EMode.sonyu, HFlag.EMode.houshi3P, HFlag.EMode.sonyu3P,
            HFlag.EMode.houshi3PMMF, HFlag.EMode.sonyu3PMMF
        }
            .Concat(playerlessModes).ToArray();

        private static readonly string[] orgasmAnimations =
        {
            "OUT_START", "OUT_LOOP", "IN_START", "IN_LOOP", "IN_Start", "IN_Loop",
            "M_OUT_Start", "M_OUT_Loop", "M_IN_Start", "M_IN_Loop",
            "WS_IN_Start", "WS_IN_Loop", "SS_IN_Start", "SS_IN_Loop",
            "A_WS_IN_Start", "A_WS_IN_Loop", "A_SS_IN_Start", "A_SS_IN_Loop", "Orgasm"
        };

        private static readonly string[] activeAnimations =
        {
            "WLoop", "MLoop", "SLoop",
            // anal
            "A_WLoop", "A_SLoop", "A_OLoop",
            // orgasm
            "OLoop", "A_OLoop"
        };

        private ObjectIDGenerator idGenerator;

        public HFlag Flags { get; private set; }

        private string Blend => AnimationConfig.TrackAnimationBlending.Value
            ? $"blend{Mathf.Round(Flags.motion)}"
            : "noblend";

        protected override int HeroineCount => Flags.lstHeroine.Count;

        protected override int MaxHeroineCount => 2;

        protected override bool IsHardSex => Flags?.nowAnimStateName?.Contains("SLoop") ?? false;

        protected override int AnimationLayer => 0;

        protected override float PenisSize => 0.1f;

        protected override float VibrationIntensity => Flags.speedCalc == 0f ? 1f : Flags.speedCalc;

        protected override MethodInfo[] StartHMethods =>
            new[] { AccessTools.Method(typeof(HFlag), nameof(HFlag.Start)) };

        protected override MethodInfo[] EndHMethods => new[]
        {
            AccessTools.Method(typeof(HSprite), nameof(HSprite.OnClickHSceneEnd)),
            AccessTools.Method(typeof(HSprite), nameof(HSprite.OnClickTrespassing))
        };

        protected override bool IsIdle(int girlIndex) => !supportedModes.Contains(Flags.mode)
            || !activeAnimations.Contains(Flags.nowAnimStateName)
            || (Flags.speed < 1f && !playerlessModes.Contains(Flags.mode));

        protected override bool IsOrgasming(int girlIndex) =>
            orgasmAnimations.Contains(Flags.nowAnimStateName);

        protected override Animator GetFemaleAnimator(int girlIndex) =>
            Flags.lstHeroine[girlIndex].chaCtrl.animBody;

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            Flags.lstHeroine[girlIndex].chaCtrl.objBodyBone;

        protected override Transform PenisBase => throw new NotImplementedException();
        
        protected override Transform[] PenisBases
        {
            get
            {
                Transform root;
                switch (Flags.mode)
                {
                    case HFlag.EMode.masturbation:
                        root = GetFemaleRoot(0).transform;
                        return new[] { root.FindLoop(FemaleBoneNames[Bone.RightHand]).transform };
                    case HFlag.EMode.lesbian:
                        root = GetFemaleRoot(1).transform;
                        return new[] { Bone.Vagina, Bone.RightHand, Bone.Mouth }
                            .Select(bone => FemaleBoneNames[bone])
                            .Select(name => root.FindLoop(name).transform)
                            .ToArray();
                    default:
                        root = Flags.player.chaCtrl.objBodyBone.transform;
                        return new[] { root.FindLoop("k_f_tamaL_00").transform };
                }
            }
        }

        protected override void GetAnimState(int girlIndex, out float normalizedTime,
            out float length, out float speed)
        {
            var state = GetAnimatorStateInfo(girlIndex);
            normalizedTime = state.normalizedTime;
            length = state.length;
            speed = 1f; // inexplicably becomes 2 during OLoop
        }

        protected override string GetPose(int girlIndex) =>
            // Sideloaded animations all have the same id and name.
            // The only surefire way to uniquely identify an animation seems
            // to be the object reference itself.
            idGenerator.GetId(Flags.nowAnimationInfo, out _)
                + "." + Flags.nowAnimationInfo.nameAnimation
                + "." + Flags.nowAnimStateName
                + "." + Blend;

        protected override IEnumerator WaitAfterPoseChange()
        {
            // play nicely with CrossFader
            while (!GetAnimatorStateInfo(0).IsName(Flags.nowAnimStateName))
            {
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.1f);
        }
        
        protected override IEnumerator UntilReady(object flags)
        {
            Flags = (HFlag)flags;
            idGenerator = new ObjectIDGenerator();
            yield return new WaitWhile(() => Flags.lstHeroine.IsNullOrEmpty()
                || Flags.lstHeroine.Any(girl => girl.chaCtrl?.animBody == null)
                || Flags.player?.chaCtrl?.animBody == null);
        }
    }
}