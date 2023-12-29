using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.DJ
{
    public class DatsuiJankenGame : GameAdapter
    {
        private static readonly int[] idleStates = { 0, 1, 9, 10 };
        private static readonly int[] orgasmStates = { 7, 8 };
        
        private GameObject female;
        private Animator femaleAnimator;
        private Traverse<int> status;
        
        protected override MethodInfo[] StartHMethods =>
            new [] { AccessTools.Method("CanvasFlag, Assembly-CSharp:MotionFlagCheck") };
    
        protected override MethodInfo[] EndHMethods =>
            new [] { AccessTools.Method("AnimeManager, Assembly-CSharp:ExitSexScene") };
        
        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "L_Vagina" },
            { Bone.Mouth, "Tongue03" },
            { Bone.RightHand, "Right_Index_Tip" },
            { Bone.LeftBreast, "L_Brest_Tip" },
            { Bone.RightBreast, "R_Brest_Tip" },
        };

        protected override Transform PenisBase => GameObject.Find("Fukuro").transform;
        protected override float PenisSize => 0.1f;
        protected override int AnimationLayer => 0;
        protected override int HeroineCount => 1;
        protected override int MaxHeroineCount => 1;
        protected override bool IsHardSex => false;

        protected override Animator GetFemaleAnimator(int girlIndex) => femaleAnimator;

        protected override GameObject GetFemaleRoot(int girlIndex) => female;

        protected override string GetPose(int girlIndex) =>
            GetAnimatorStateInfo(0).fullPathHash.ToString();

        protected override bool IsIdle(int girlIndex) => idleStates.Contains(status.Value);

        protected override bool IsOrgasming(int girlIndex) => orgasmStates.Contains(status.Value);

        protected override IEnumerator UntilReady(object instance)
        {
            yield return new WaitForSeconds(1f);
            female = GameObject.Find("Sonoe (1)");
            femaleAnimator = female.GetComponent<Animator>();
            status = Traverse.Create(female.GetComponent("VoiceObject")).Field<int>("status");
        }
    }
}