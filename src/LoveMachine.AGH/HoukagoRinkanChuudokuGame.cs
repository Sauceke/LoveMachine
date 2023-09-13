using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Game;
using LoveMachine.Core.Common;
using UnityEngine;

namespace LoveMachine.AGH
{
    internal sealed class HoukagoRinkanChuudokuGame : GameAdapter
    {
        private static readonly Dictionary<Bone, string> sayaBones = new Dictionary<Bone, string>
        {
            { Bone.Vagina, "HS01_cli" },
            { Bone.Anus, "HS01_anaru" },
            { Bone.Mouth, "BF01_tongue01" },
            { Bone.LeftHand, "bip01 L Finger1Nub" },
            { Bone.RightHand, "bip01 R Finger1Nub" },
            { Bone.LeftBreast, "HS_Breast_LL" },
            { Bone.RightBreast, "HS_Breast_RR" }
        };

        private static readonly Dictionary<Bone, string> elenaBones =
            sayaBones.ToDictionary(kvp => kvp.Key, kvp => kvp.Value + "_02");

        private static readonly string[] penisBaseNames =
            { "BP00_tamaL", "BP00_tamaL_mobA", "BP00_tamaL_mobB", "BP00_tamaL_mobC" };

        private GameObject femaleRoot;
        private Animator femaleAnimator;
        private Transform coom;
        private Traverse<int> mode;

        protected override Dictionary<Bone, string> FemaleBoneNames =>
            GameObject.Find("CH01") != null ? sayaBones : elenaBones;

        protected override int HeroineCount => 1;

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => true;

        protected override int AnimationLayer => 0;

        protected override float PenisSize => 0.5f;

        protected override float MinOrgasmDurationSecs => 6f;

        protected override MethodInfo[] StartHMethods => new[]
        {
            AccessTools.Method("FH_AnimeController, Assembly-CSharp:Start"),
            AccessTools.Method("RI_AnimeController, Assembly-CSharp:Start")
        };

        protected override MethodInfo[] EndHMethods => new[]
        {
            AccessTools.Method("FH_SetUp, Assembly-CSharp:Unload"),
            AccessTools.Method("RI_SetUp, Assembly-CSharp:Unload")
        };

        protected override Animator GetFemaleAnimator(int girlIndex) => femaleAnimator;

        protected override GameObject GetFemaleRoot(int girlIndex) => femaleRoot;

        protected override Transform PenisBase => throw new NotImplementedException();

        protected override Transform[] PenisBases => penisBaseNames
            .Select(name => GameObject.Find(name)?.transform)
            .Where(transform => transform != null)
            .ToArray();

        protected override string GetPose(int girlIndex) =>
            femaleAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        protected override bool IsIdle(int girlIndex) => mode.Value == 4;

        protected override bool IsOrgasming(int girlIndex) => coom.localPosition != Vector3.zero;

        protected override void OnStartH(object animeController) =>
            mode = Traverse.Create(animeController).Field<int>("Mode");

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSeconds(5f);
            femaleRoot = GameObject.Find("CH01/CH0001") ?? GameObject.Find("CH02/CH0002");
            femaleAnimator = femaleRoot.GetComponent<Animator>();
            coom = GameObject.Find("PC01/PC/HS01_SE04").transform;
        }
    }
}