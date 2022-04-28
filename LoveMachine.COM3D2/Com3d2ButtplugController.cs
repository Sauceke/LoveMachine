using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.COM3D2
{
    internal abstract class Com3d2ButtplugController : ButtplugController
    {
        private const string SpineF = "Bip01/Bip01 Spine/Bip01 Spine0a/" +
                        "Bip01 Spine1/Bip01 Spine1a";
        private const string PelvisF = "Bip01/Bip01 Pelvis";

        private readonly string[] idlePoseNames = { "taiki", "nade", "shaseigo" };

        protected override int HeroineCount => FindCharaObject($"Maid[1]") == null ? 1 : 2;

        // TOOD animation name numbering is not consistent, need to make some sense out of it
        protected override bool IsHardSex => GetPose(0).Contains('2');

        protected override bool IsHSceneInterrupted => false;

        protected override int AnimationLayer => throw new NotImplementedException();

        protected override CustomYieldInstruction WaitAfterPoseChange =>
            new WaitForSecondsRealtime(1f);

        protected override Animator GetFemaleAnimator(int girlIndex) =>
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

        private AnimationState GetActiveState() => FindObjectsOfType<Animation>()
            .Where(animation => animation.name == "_BO_mbody")
            .SelectMany(animation => animation.Cast<AnimationState>()
                .Where(state => animation.IsPlaying(state.name)))
            .OrderBy(state => state.length)
            .ThenBy(state => state.name)
            .FirstOrDefault();

        protected override Dictionary<Bone, Transform> GetFemaleBones(int girlIndex)
        {
            var maid = FindCharaObject($"Maid[{girlIndex}]");
            return new Dictionary<Bone, Transform>
            {
                { Bone.Vagina, FindBoneByPath(maid, PelvisF + "/_IK_vagina") },
                {
                    Bone.RightHand,
                    FindBoneByPath(maid, SpineF + "/Bip01 R Clavicle/Bip01 R UpperArm/" +
                        "Bip01 R Forearm/Bip01 R Hand")
                },
                {
                    Bone.Mouth,
                    FindBoneByPath(maid, SpineF + "/Bip01 Neck/Bip01 Head/_SM_face007/MouthUp")
                },
                { Bone.LeftBreast, FindBoneByPath(maid, SpineF + "/Mune_L/_IK_muneL") },
                { Bone.RightBreast, FindBoneByPath(maid, SpineF + "/Mune_R/_IK_muneR") },
                {
                    Bone.RightFoot,
                    FindBoneByPath(maid, PelvisF +
                        "/Bip01 L Thigh/Bip01 L Calf/Bip01 L Foot/Bip01 L Toe0")
                },
                {
                    Bone.LeftFoot,
                    FindBoneByPath(maid, PelvisF +
                        "/Bip01 R Thigh/Bip01 R Calf/Bip01 R Foot/Bip01 R Toe0")
                }
            };
        }

        protected override Transform GetMaleBone() => FindBoneByPath(FindCharaObject("Man[0]"),
                "ManBip/ManBip Pelvis/chinkoCenter/tamabukuro");

        private static Transform FindCharaObject(string pattern) =>
            GameObject.Find("__GameMain__/Character/Active/AllOffset").transform.Cast<Transform>()
                .Where(child => child?.gameObject?.name?.StartsWith(pattern) ?? false)
                .Select(child => child.Find("Offset")?.GetChild(0))
                .FirstOrDefault();

        private static Transform FindBoneByPath(Transform character, string path)
        {
            // Find the root character object
            var bone = character.transform.Find(path)
                // If the program can not find the component, it will try to use the name of the
                // component to match every child of the root chara by recursion
                ?? FindDeepChildByName(character.transform, path.Split('/').Last());
            CoreConfig.Logger.LogDebug($"Requested path: {path}, " +
                $"found path: {GetGameObjectPath(bone.gameObject)}");
            return bone;
        }

        private static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }

        public static Transform FindDeepChildByName(Transform transform, string name) =>
            transform.gameObject.GetComponentsInChildren<Transform>()
                .Where(child => child.name == name).FirstOrDefault();

        protected override string GetPose(int girlIndex) => GetActiveState()?.name;

        protected override bool IsIdle(int girlIndex)
        {
            string pose = GetPose(girlIndex);
            return idlePoseNames.Any(pose.Contains);
        }

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSecondsRealtime(5f);
        }
    }

    internal class Com3d2ButtplugVibeController : Com3d2ButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone) =>
            RunVibratorLoop(girlIndex, bone);
    }

    internal class Com3d2ButtplugStrokerController : Com3d2ButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone) =>
            RunStrokerLoop(girlIndex, bone);
    }
}
