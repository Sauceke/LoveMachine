using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.KKLB
{
    public class KoiKoiGame : GameDescriptor
    {
        private Animator animator;
        private TimeUnlooper unlooper;
        
        protected override MethodInfo[] StartHMethods => new[]
        {
            AccessTools.Method("sv08.CostumeSelectScript, Assembly-CSharp:StartAdultMode")
        };

        protected override MethodInfo[] EndHMethods => new MethodInfo[] { };

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "HideSurface_FF" },
            { Bone.LeftHand, "LeftHandIndex_02" },
            { Bone.RightHand, "RightHandIndex_02" }
        };

        protected override Transform PenisBase => GameObject
            .Find("CharacterManager/M01_player_combine/BaseBone/Root/Hips_00/Balls_00/Balls_01")?
            .transform;

        protected override int AnimationLayer => 0;
        
        protected override int HeroineCount => 1;
        
        protected override int MaxHeroineCount => 1;
        
        protected override bool IsHardSex => false;

        protected override Animator GetFemaleAnimator(int girlIndex) =>
            throw new NotImplementedException();

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            GameObject.Find("CharacterManager/F02_umeko_combine/BaseBone");

        protected override void GetAnimState(int girlIndex, out float normalizedTime, out float length, out float speed)
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            normalizedTime = unlooper.LoopingToMonotonic(state.normalizedTime);
            length = state.length;
            speed = state.speed;
        }

        protected override string GetPose(int girlIndex) =>
            animator.GetCurrentAnimatorStateInfo(0).fullPathHash.ToString();

        protected override bool IsIdle(int girlIndex) => false;
        
        protected override IEnumerator UntilReady()
        {
            while (PenisBase == null || GetFemaleRoot(0) == null)
            {
                yield return new WaitForSeconds(1f);
            }
            animator = GetFemaleRoot(0).GetComponent<Animator>();
            unlooper = new TimeUnlooper();
            yield return null;
        }
    }
}