using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.SG
{
    public class SexaroidGirlGame: GameAdapter
    {
        private static readonly string[] sexLayerNames =
        {
            "fera", "kuni", "kuni_upper", "seijou", "seijou_upper", "kouhai", "kouhai_upper",
            "kijou", "kijou_upper"
        };
        
        private int[] sexLayers;
        private GameObject girl_Prefab;
        private Animator girlAC;
        private Traverse<string> _taii;
        private Traverse<bool> girl_idling;
        private Traverse<bool> shasei_now;
        private Traverse<bool> autoPiston;

        protected override MethodInfo[] StartHMethods =>
            new[] { AccessTools.Method("AnimatorControl3, Assembly-CSharp:_change_Taii") };
        
        protected override MethodInfo[] EndHMethods => new MethodInfo[] {};
        
        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "kr_root_end" },
            { Bone.Mouth, "mouth_b_end" },
            { Bone.RightHand, "yubi_2_str_R_end" }
        };

        protected override Transform PenisBase => GameObject.Find("DEF-tama_L_end").transform;
        protected override float PenisSize => 0.08f;
        protected override int AnimationLayer => sexLayers.OrderBy(girlAC.GetLayerWeight).Last();
        protected override int HeroineCount => 1;
        protected override int MaxHeroineCount => 1;
        protected override bool IsHardSex => false;

        protected override Animator GetFemaleAnimator(int girlIndex) => girlAC;

        protected override GameObject GetFemaleRoot(int girlIndex) => girl_Prefab;

        protected override string GetPose(int girlIndex) =>
            $"{_taii.Value}.{AnimationLayer}.{autoPiston.Value}";

        protected override bool IsIdle(int girlIndex) => girl_idling.Value;

        protected override bool IsOrgasming(int girlIndex) => shasei_now.Value;

        protected override IEnumerator UntilReady(object instance)
        {
            yield return new WaitForSeconds(5f);
            var script = Traverse.Create(instance);
            girl_Prefab = script.Field<GameObject>("girl_Prefab").Value;
            girlAC = script.Field<Animator>("girlAC").Value;
            _taii = script.Field<string>("_taii");
            girl_idling = script.Field<bool>("girl_idling");
            shasei_now = script.Field<bool>("shasei_now");
            autoPiston = script.Field<bool>("autoPiston");
            sexLayers = sexLayerNames.Select(girlAC.GetLayerIndex).ToArray();
            while (!autoPiston.Value)
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
}