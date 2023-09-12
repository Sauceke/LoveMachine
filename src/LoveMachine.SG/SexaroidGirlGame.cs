using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.SG
{
    public class SexaroidGirlGame: GameAdapter
    {
        private GameObject girl_Prefab;
        private Animator girlAC;
        private Traverse<string> _taii;
        private Traverse<float> now_blend;
        private Traverse<bool> girl_idling;

        protected override MethodInfo[] StartHMethods => new[]
        {
            AccessTools.Method("AnimatorControl3, Assembly-CSharp:_change_Taii")
        };
        
        protected override MethodInfo[] EndHMethods => new MethodInfo[] {};
        
        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "kr_root_end" },
            { Bone.RightHand, "yubi_2_str_R_end" }
        };

        protected override Transform PenisBase => GameObject.Find("DEF-tama_L_end").transform;
        protected override int AnimationLayer => 0;
        protected override int HeroineCount => 1;
        protected override int MaxHeroineCount => 1;
        protected override bool IsHardSex => false;

        protected override Animator GetFemaleAnimator(int girlIndex) => girlAC;

        protected override GameObject GetFemaleRoot(int girlIndex) => girl_Prefab;

        protected override string GetPose(int girlIndex) => $"{_taii.Value}.{now_blend.Value}";

        protected override bool IsIdle(int girlIndex) => girl_idling.Value;

        protected override void SetStartHInstance(object instance)
        {
            var script = Traverse.Create(instance);
            girl_Prefab = script.Field<GameObject>("girl_Prefab").Value;
            girlAC = script.Field<Animator>("girlAC").Value;
            _taii = script.Field<string>("_taii");
            now_blend = script.Field<float>("now_blend");
            girl_idling = script.Field<bool>("girl_idling");
        }

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSeconds(5f);
        }
    }
}