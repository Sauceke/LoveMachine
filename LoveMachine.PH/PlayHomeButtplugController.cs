using System.Collections;
using System.Collections.Generic;
using System.Linq;
using H;
using IllusionUtility.GetUtility;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.PH
{
    internal abstract class PlayHomeButtplugController : ButtplugController
    {
        internal const string MaleBoneName = "k_m_tamaC_00";

        internal static readonly Dictionary<Bone, string> femaleBones = new Dictionary<Bone, string>
        {
            { Bone.LeftBreast, "k_f_munenipL_00" },
            { Bone.RightBreast, "k_f_munenipR_00" },
            { Bone.Vagina, "k_f_kokan_00" },
            { Bone.Mouth, "cf_J_MouthCavity" },
            { Bone.LeftHand, "cf_J_Hand_Index01_L" },
            { Bone.RightHand, "cf_J_Hand_Index01_R" },
            { Bone.LeftFoot, "k_f_toeL_00" },
            { Bone.RightFoot, "k_f_toeR_00" },
        };

        private static readonly H_STATE[] activeHStates = { H_STATE.LOOP, H_STATE.SPURT };

        protected H_Scene scene;

        protected override int HeroineCount => scene.mainMembers.females.Count;

        protected override bool IsHardSex => true;

        protected override int AnimationLayer => 0;

        protected override bool IsHSceneInterrupted => false;

        protected override float PenisSize => 0.2f;

        protected override Animator GetFemaleAnimator(int girlIndex) =>
            scene.mainMembers.females[girlIndex].body.Anime;

        protected override Dictionary<Bone, Transform> GetFemaleBones(int girlIndex)
        {
            var bodyBone = scene.mainMembers.females[girlIndex].objBodyBone.transform;
            return femaleBones.ToDictionary(kvp => kvp.Key,
                kvp => bodyBone.FindLoop(kvp.Value).transform);
        }

        protected override Transform GetMaleBone()
        {
            var bodyBone = scene.mainMembers.males[0].objBodyBone.transform;
            return bodyBone.FindLoop(MaleBoneName).transform;
        }

        protected override string GetPose(int girlIndex) =>
            scene.mainMembers.StyleData == null
                ? "none"
                : scene.mainMembers.StyleData.id + "." +
                    GetFemaleAnimator(girlIndex).GetCurrentAnimatorStateInfo(0).fullPathHash;

        protected override bool IsIdle(int _) =>
            !activeHStates.Contains(scene.mainMembers.StateMgr.nowStateID);

        internal void OnStartH(H_Scene scene)
        {
            this.scene = scene;
            OnStartH();
        }

        protected override IEnumerator UntilReady()
        {
            while (scene.mainMembers?.StyleData == null
                || scene.mainMembers.females.IsNullOrEmpty()
                || scene.mainMembers.males.IsNullOrEmpty()
                || scene.mainMembers.females[0] == null
                || scene.mainMembers.males[0] == null)
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    internal class PlayHomeButtplugVibrationController : PlayHomeButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone) =>
            RunVibratorLoop(girlIndex, bone);
    }

    internal class PlayHomeButtplugStrokerController : PlayHomeButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone) =>
            RunStrokerLoop(girlIndex, bone);
    }

    internal class PlayHomeButtplugRotatorController : PlayHomeButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone) =>
            RunRotatorLoop(girlIndex, bone);
    }
}
