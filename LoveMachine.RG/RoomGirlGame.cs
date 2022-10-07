using System;
using System.Collections;
using System.Collections.Generic;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.RG
{
    internal class RoomGirlGame : GameDescriptor
    {
        private Animator femaleAnimator;

        public RoomGirlGame(IntPtr handle) : base(handle) { }

        public override int AnimationLayer => 0;

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "cf_J_Kokan" },
            { Bone.RightHand, "cf_J_Hand_Wrist_s_R" },
            { Bone.LeftHand, "cf_J_Hand_Wrist_s_L" },
            { Bone.RightBreast, "cf_J_Mune04_s_R" },
            { Bone.LeftBreast, "cf_J_Mune04_s_L" },
            { Bone.Mouth, "cf_J_MouthCavity" },
            { Bone.RightFoot, "cf_J_Toes01_L" },
            { Bone.LeftFoot, "cf_J_Toes01_R" }
        };

        protected override int HeroineCount => 1;

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => false;

        protected override bool IsHSceneInterrupted => false;

        public override Animator GetFemaleAnimator(int girlIndex) => femaleAnimator;

        protected override Transform GetDickBase() => GameObject.Find("cm_J_dan_f_L").transform;

        protected override GameObject GetFemaleRoot(int girlIndex) => GameObject.Find($"chaF_001");

        protected override string GetPose(int girlIndex) =>
            femaleAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash.ToString();

        protected override bool IsIdle(int girlIndex) => false;

        protected override IEnumerator UntilReady()
        {
            yield return new WaitWhile((Il2CppSystem.Func<bool>)(() => GetFemaleRoot(0) == null));
            yield return new WaitForSeconds(5f);
            femaleAnimator = GameObject.Find("chaF_001/BodyTop/p_cf_anim").GetComponent<Animator>();
        }
    }
}
