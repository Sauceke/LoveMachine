using System.Collections;
using System.Collections.Generic;
using System.Linq;
using H;
using IllusionUtility.GetUtility;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.PH
{
    public abstract class PlayHomeButtplugController : ButtplugController
    {
        internal const string MaleBoneName = "k_m_tamaC_00";

        internal static readonly Dictionary<string, string> femaleBones
            = new Dictionary<string, string>
        {
            { "k_f_munenipL_00", "Left Breast"},
            { "k_f_munenipR_00", "Right Breast"},
            { "k_f_kokan_00", "Pussy"},
            { "cf_J_MouthCavity", "Mouth" },
            { "cf_J_Hand_Index01_L", "Left Hand"},
            { "cf_J_Hand_Index01_R", "Right Hand"},
            { "k_f_toeL_00", "Left Foot"},
            { "k_f_toeR_00", "Right Foot"}
        };

        private static readonly List<H_STATE> activeHStates
            = new List<H_STATE> { H_STATE.LOOP, H_STATE.SPURT };

        protected H_Scene scene;

        protected override int HeroineCount => scene.mainMembers.females.Count;

        protected override bool IsHardSex => true;

        protected override int AnimationLayer => 0;

        protected override bool IsHSceneInterrupted => false;

        protected override Animator GetFemaleAnimator(int girlIndex) =>
            scene.mainMembers.females[girlIndex].body.Anime;

        protected override List<Transform> GetFemaleBones(int girlIndex)
        {
            var bodyBone = scene.mainMembers.females[girlIndex].objBodyBone.transform;
            return femaleBones.Keys.Select(name => bodyBone.FindLoop(name).transform).ToList();
        }

        protected override Transform GetMaleBone()
        {
            var bodyBone = scene.mainMembers.males[0].objBodyBone.transform;
            return bodyBone.FindLoop(MaleBoneName).transform;
        }

        protected override string GetPose(int girlIndex)
        {
            if (scene.mainMembers.StyleData == null)
            {
                return "none";
            }
            return scene.mainMembers.StyleData.id + "."
                + GetFemaleAnimator(girlIndex).GetCurrentAnimatorStateInfo(0).fullPathHash + "."
                + girlIndex;
        }

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

    public class PlayHomeButtplugVibrationController : PlayHomeButtplugController
    {
        protected override IEnumerator Run(int girlIndex, int boneIndex)
            => RunVibratorLoop(girlIndex, boneIndex);
    }

    public class PlayHomeButtplugStrokerController : PlayHomeButtplugController
    {
        protected override IEnumerator Run(int girlIndex, int boneIndex)
            => RunStrokerLoop(girlIndex, boneIndex);
    }
}
