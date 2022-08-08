using System.Collections;
using System.Collections.Generic;
using System.Linq;
using H;
using IllusionUtility.GetUtility;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.PH
{
    internal sealed class PlayHomeGame : GameDescriptor
    {
        private static readonly H_STATE[] activeHStates = { H_STATE.LOOP, H_STATE.SPURT };

        private H_Scene scene;

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
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

        protected override int HeroineCount => scene.mainMembers.females.Count;

        protected override int MaxHeroineCount => 2;

        protected override bool IsHardSex => true;

        public override int AnimationLayer => 0;

        protected override bool IsHSceneInterrupted => false;

        protected override float PenisSize => 0.2f;

        public override Animator GetFemaleAnimator(int girlIndex) =>
            scene.mainMembers.females[girlIndex].body.Anime;

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            scene.mainMembers.females[girlIndex].objBodyBone;

        protected override Transform GetDickBase() =>
            scene.mainMembers.males[0].objBodyBone.transform.FindLoop("k_m_tamaC_00").transform;

        protected override string GetPose(int girlIndex) =>
            (scene.mainMembers.StyleData?.id ?? "none")
                + "." + GetAnimatorStateInfo(girlIndex).fullPathHash;

        protected override bool IsIdle(int _) =>
            !activeHStates.Contains(scene.mainMembers.StateMgr.nowStateID);

        internal void OnStartH(H_Scene scene)
        {
            this.scene = scene;
            StartH();
        }

        protected override IEnumerator UntilReady()
        {
            yield return new WaitWhile(() => scene.mainMembers?.StyleData == null
                || scene.mainMembers.females.IsNullOrEmpty()
                || scene.mainMembers.males.IsNullOrEmpty()
                || scene.mainMembers.females[0] == null
                || scene.mainMembers.males[0] == null);
        }
    }
}
