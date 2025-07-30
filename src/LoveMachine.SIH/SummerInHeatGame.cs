using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.SIH
{
    internal class SummerInHeatGame : GameAdapter
    {
        private static readonly int[] activeSteps = { 1, 2, 5 };

        private GameObject[] femaleRoots;
        private Animator[] femaleAnimators;
        private Traverse<int> step;

        protected override MethodInfo[] StartHMethods => new[]
        {
            AccessTools.Method("FH_Controller, Assembly-CSharp:Start_Method"),
            AccessTools.Method("Talk_Controller, Assembly-CSharp:Start_Method")
        };

        protected override MethodInfo[] EndHMethods =>
            new[] { AccessTools.Method("FadeManager_GUI, Assembly-CSharp:TransScene") };

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "HS_cli" },
            { Bone.Anus, "HS_anaruB01" },
            { Bone.LeftHand, "bip01 L Finger1Nub" },
            { Bone.RightHand, "bip01 R Finger1Nub" },
            { Bone.Mouth, "HS_mouth01" },
            { Bone.LeftBreast, "BP_Breast_L03" },
            { Bone.RightBreast, "BP_Breast_R03" },
            { Bone.LeftFoot, "bip01 L Toe4Nub" },
            { Bone.RightFoot, "bip01 R Toe4Nub" }
        };

        protected override Transform PenisBase => throw new NotImplementedException();

        protected override Transform[] PenisBases =>
            new[] { GameObject.Find("HS01_penis11_PC01"), GameObject.Find("HS00_penis01_PC03") }
                .Where(go => go != null && go.activeInHierarchy)
                .Select(go => go.transform).ToArray();

        protected override float PenisSize => 0.12f;
        protected override int AnimationLayer => 0;
        protected override int HeroineCount => femaleRoots.Length;
        protected override int MaxHeroineCount => 2;
        protected override bool IsHardSex => true;

        protected override Animator GetFemaleAnimator(int girlIndex) => femaleAnimators[girlIndex];

        protected override GameObject GetFemaleRoot(int girlIndex) => femaleRoots[girlIndex];

        protected override string GetPose(int girlIndex) =>
            femaleAnimators[girlIndex].GetCurrentAnimatorStateInfo(0).fullPathHash.ToString();

        protected override bool IsIdle(int girlIndex) => !activeSteps.Contains(step.Value);

        protected override bool IsOrgasming(int girlIndex) => step.Value == 3;

        protected override IEnumerator UntilReady(object instance)
        {
            yield return new WaitForSeconds(5f);
            var controller = Traverse.Create(instance);
            femaleRoots = new[]
                {
                    GameObject.Find("CH_Prefub_A/CHbase"),
                    GameObject.Find("CH_Prefub_B/CHbase")
                }
                .Where(go => go != null && go.activeInHierarchy)
                .ToArray();
            femaleAnimators = femaleRoots
                .Select(root => root.GetComponent<Animator>())
                .ToArray();
            step = instance.GetType().Name == "FH_Controller"
                ? controller.Field<int>("FH_Step")
                : controller.Field<int>("TK_Step");
        }
    }
}