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
        private Traverse<GameObject> root;
        private Traverse<bool> busy;
        private TimeUnlooper unlooper;
        
        private Animator Animator => root.Value.GetComponent<Animator>();
        
        protected override MethodInfo[] StartHMethods =>
            new[] { AccessTools.Method("sv08.AdultManager, Assembly-CSharp:Start") };

        protected override MethodInfo[] EndHMethods =>
            new[] { AccessTools.Method("sv08.AdultManager, Assembly-CSharp:OnDestroy") };

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "HideSurface_FF" },
            { Bone.LeftHand, "LeftHandIndex_02" },
            { Bone.RightHand, "RightHandIndex_02" }
        };

        protected override Transform PenisBase => GameObject
            .Find("CharacterManager/M01_player_combine/BaseBone/Root/Hips_00/Balls_00/Balls_01")?
            .transform;

        protected override float PenisSize => 0.1f;

        protected override float MinStrokeLength => 0.3f;

        protected override int AnimationLayer => 0;
        
        protected override int HeroineCount => 1;
        
        protected override int MaxHeroineCount => 1;
        
        protected override bool IsHardSex => false;

        protected override Animator GetFemaleAnimator(int girlIndex) =>
            throw new NotImplementedException();

        protected override GameObject GetFemaleRoot(int girlIndex) => root.Value;

        protected override void GetAnimState(int girlIndex, out float normalizedTime, out float length, out float speed)
        {
            var state = Animator.GetCurrentAnimatorStateInfo(0);
            normalizedTime = unlooper.LoopingToMonotonic(state.normalizedTime);
            length = state.length;
            speed = state.speed;
        }

        protected override string GetPose(int girlIndex) =>
            Animator.GetCurrentAnimatorStateInfo(0).fullPathHash.ToString();

        protected override bool IsIdle(int girlIndex) => !busy.Value;

        protected override void SetStartHInstance(object instance)
        {
            var adultManager = Traverse.Create(instance);
            root = adultManager.Field<GameObject>("PartnerBase");
            busy = adultManager.Field<bool>("bBusy");
            unlooper = new TimeUnlooper();
        }
        
        protected override IEnumerator UntilReady()
        {
            while (PenisBase == null)
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
}