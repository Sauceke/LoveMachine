using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.WP
{
    public class WrithingPlayGame: GameAdapter
    {
        private Traverse ai;
        private GameObject ChrA;
        private GameObject ChrB;
        private Traverse<string> TypeAI;
        private Traverse<bool> PlayAuto; 
        private Traverse<bool> IsAutoSceneChange;
        private Traverse<int> CurrentPose;
        private Traverse<float> autoMouseCount;
        private Traverse<float> autoSpeed;
        private Traverse<float> autoSpeedTurn;
        private Traverse<float> autoSpeedScale;
        
        protected override MethodInfo[] StartHMethods =>
            new [] { AccessTools.Method("ai, Assembly-UnityScript:Start") };
        
        protected override MethodInfo[] EndHMethods =>
            new MethodInfo[] { };
        
        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "point_Penis" },
            { Bone.Mouth, "point_MouthCenter" },
            { Bone.RightHand, "joint_RightIndex3Tip" }
        };

        protected override Transform PenisBase => FindDeepChildrenByName(ChrB, "point_Penis")[0];
        protected override int AnimationLayer => throw new NotImplementedException();
        protected override int HeroineCount => 1;
        protected override int MaxHeroineCount => 1;
        protected override bool IsHardSex => TypeAI.Value == "modae";

        protected override Animator GetFemaleAnimator(int girlIndex) =>
            throw new NotImplementedException();

        protected override GameObject GetFemaleRoot(int girlIndex) => ChrA;

        protected override string GetPose(int girlIndex) =>
            CurrentPose.Value + "." + PlayAuto.Value;

        protected override bool IsIdle(int girlIndex) => !PlayAuto.Value || IsAutoSceneChange.Value;

        protected override void SetStartHInstance(object instance) =>
            ai = Traverse.Create(instance);

        // this is the most wtf code i've ever modded ;_;
        protected override void GetAnimState(int girlIndex, out float normalizedTime,
            out float length, out float speed)
        {
            normalizedTime = autoMouseCount.Value * autoSpeedScale.Value / Mathf.PI / 2;
            length = Mathf.PI * 2;
            // this isn't even the correct speed, but it's the closest I could manage
            speed = autoSpeed.Value * autoSpeedTurn.Value * autoSpeedScale.Value;
        }

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSeconds(5f);
            var gs = ai.Field("gls");
            ChrA = gs.Field<GameObject>("ChrA").Value;
            ChrB = gs.Field<GameObject>("ChrB").Value;
            TypeAI = gs.Field<string>("TypeAI");
            PlayAuto = gs.Field<bool>("PlayAuto");
            CurrentPose = gs.Field<int>("CurrentPose");
            IsAutoSceneChange = gs.Field<bool>("IsAutoSceneChange");
            autoMouseCount = ai.Field<float>("autoMouseCount");
            autoSpeed = ai.Field<float>("autoSpeed");
            autoSpeedTurn = ai.Field<float>("autoSpeedTurn");
            autoSpeedScale = ai.Field<float>("autoSpeedScale");
        }
    }
}