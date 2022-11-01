using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.COM3D2
{
    internal sealed class Com3d2Game : GameDescriptor
    {
        private const string SpineF = "Bip01/Bip01 Spine/Bip01 Spine0a/Bip01 Spine1/Bip01 Spine1a";
        private const string PelvisF = "Bip01/Bip01 Pelvis";

        private readonly string[] idlePoseNames = { "taiki", "nade", "shaseigo" };
        private readonly string[] climaxPoseNames = { "shasei_", "zeccyou_" };

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, PelvisF + "/_IK_vagina" },
            {
                Bone.RightHand,
                SpineF + "/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand"
            },
            {
                Bone.Mouth,
                SpineF + "/Bip01 Neck/Bip01 Head/_SM_face007/MouthUp"
            },
            { Bone.LeftBreast, SpineF + "/Mune_L/_IK_muneL" },
            { Bone.RightBreast, SpineF + "/Mune_R/_IK_muneR" },
            {
                Bone.RightFoot,
                PelvisF +  "/Bip01 L Thigh/Bip01 L Calf/Bip01 L Foot/Bip01 L Toe0"
            },
            {
                Bone.LeftFoot,
                PelvisF +  "/Bip01 R Thigh/Bip01 R Calf/Bip01 R Foot/Bip01 R Toe0"
            }
        };

        protected override int HeroineCount => FindCharaObject("Maid[1]") == null ? 1 : 2;

        protected override int MaxHeroineCount => 2;

        // TOOD animation name numbering is not consistent, need to make some sense out of it
        protected override bool IsHardSex => GetPose(0).Contains('2');

        protected override bool IsHSceneInterrupted => false;

        public override int AnimationLayer => throw new NotImplementedException();

        protected override IEnumerator WaitAfterPoseChange()
        {
            yield return new WaitForSeconds(1f);
        }

        public override Animator GetFemaleAnimator(int girlIndex) =>
            throw new NotImplementedException();

        private float lastPartialTime = 0f;
        private int totalTime = 0;

        protected override void GetAnimState(int girlIndex, out float normalizedTime,
            out float length, out float speed)
        {
            var state = GetActiveState();
            float partialTime = state.normalizedTime;
            // Yes, this is horrible. So is COM3D2's code, so I don't care.
            if (partialTime < lastPartialTime)
            {
                totalTime += Mathf.CeilToInt(lastPartialTime - partialTime);
            }
            normalizedTime = partialTime + totalTime;
            length = state.length;
            speed = state.speed;
            lastPartialTime = partialTime;
            return;
        }

        private AnimationState GetActiveState() => FindCharaObject("Man[0]")
            .GetComponentsInChildren<Animation>()
            .SelectMany(animation => animation.Cast<AnimationState>()
                .Where(state => animation.IsPlaying(state.name)))
            .OrderBy(state => state.length)
            .ThenBy(state => state.name)
            .FirstOrDefault();

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            FindCharaObject($"Maid[{girlIndex}]");

        protected override Transform GetDickBase() => FindBoneByPath(FindCharaObject("Man[0]"),
            "ManBip/ManBip Pelvis/chinkoCenter/tamabukuro");

        private static GameObject FindCharaObject(string pattern) =>
            GameObject.Find("__GameMain__/Character/Active/AllOffset").transform.Cast<Transform>()
                .Where(child => child?.gameObject?.name?.StartsWith(pattern) ?? false)
                .Select(child => child.Find("Offset")?.GetChild(0))
                .FirstOrDefault()?
                .gameObject;

        protected override string GetPose(int girlIndex) => GetActiveState()?.name;

        protected override bool IsIdle(int girlIndex) =>
            idlePoseNames.Any(GetPose(girlIndex).Contains);

        protected override bool IsOrgasming(int girlIndex) =>
            climaxPoseNames.Any(GetPose(girlIndex).Contains);

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSeconds(5f);
        }
    }
}
