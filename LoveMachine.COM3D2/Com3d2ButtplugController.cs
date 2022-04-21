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
        private const string SpineF = "Offset/_BO_body001/Bip01/Bip01 Spine/Bip01 Spine0a/" +
                        "Bip01 Spine1/Bip01 Spine1a";
        private const string PelvisF = "Offset/_BO_body001/Bip01/Bip01 Pelvis";

        protected override int HeroineCount => 1;

        protected override bool IsHardSex => GetPose(0).Contains('2');

        protected override bool IsHSceneInterrupted => false;

        protected override int AnimationLayer => throw new NotImplementedException();

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

        private AnimationState GetActiveState()
        {
            var animations = FindObjectsOfType<Animation>()
                .Where(animation => animation.name == "_BO_mbody");
            foreach (var animation in animations)
            {
                foreach (AnimationState state in animation)
                {
                    if (animation.IsPlaying(state.name))
                    {
                        return state;
                    }
                }
            }
            return null;
        }

        protected override Dictionary<Bone, Transform> GetFemaleBones(int girlIndex)
            => new Dictionary<Bone, Transform>
            {
                { Bone.Vagina, GameObject.Find(PelvisF + "/_IK_vagina").transform },
                {
                    Bone.RightHand,
                    GameObject.Find(SpineF + "/Bip01 R Clavicle/Bip01 R UpperArm/" +
                        "Bip01 R Forearm/Bip01 R Hand/_IK_handR").transform
                },
                {
                    Bone.Mouth,
                    GameObject.Find(SpineF + "/Bip01 Neck/Bip01 Head/_SM_face007/MouthUp").transform
                },
                { Bone.LeftBreast, GameObject.Find(SpineF + "/Mune_L/_IK_muneL").transform },
                { Bone.RightBreast, GameObject.Find(SpineF + "/Mune_R/_IK_muneR").transform },
                {
                    Bone.RightFoot,
                    GameObject.Find(PelvisF + "/Bip01 L Thigh/Bip01 L Calf/_IK_calfL").transform
                },
                {
                    Bone.LeftFoot,
                    GameObject.Find(PelvisF + "/Bip01 R Thigh/Bip01 R Calf/_IK_calfR").transform
                }
            };

        protected override Transform GetMaleBone() =>
            GameObject.Find("Offset/_BO_mbody/ManBip/ManBip Pelvis/chinkoCenter/tamabukuro")
                .transform;

        protected override string GetPose(int girlIndex) => GetActiveState()?.name;

        protected override bool IsIdle(int girlIndex)
        {
            string pose = GetPose(girlIndex);
            return pose.Contains("taiki") || pose.Contains("nade");
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
