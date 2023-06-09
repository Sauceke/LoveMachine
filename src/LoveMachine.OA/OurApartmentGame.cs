using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.OA
{
    internal sealed class OurApartmentGame : GameAdapter
    {
        private static readonly string[] layerNames =
            { "Base SexSim", "From Behind SexSim", "Couch Missionary SexSim" };

        private Traverse<bool> isSex;
        private Animator naomiAnimator;
        private IEnumerable<int> animationLayers;

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "cc_pussy_clit" },
            { Bone.LeftBreast, "cc_boob.l" },
            { Bone.RightBreast, "cc_boob.r" },
            { Bone.Mouth, "c_teeth_top.x" },
            { Bone.LeftFoot, "c_toes_thumb1.l" },
            { Bone.RightFoot, "c_toes_thumb1.r" },
            { Bone.LeftHand, "index1.l" },
            { Bone.RightHand, "index1.r" }
        };

        protected override int HeroineCount => 1; // Just Naomi

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => GetPose(0).Contains("Pump2");

        protected override int AnimationLayer => animationLayers
            .Where(i => naomiAnimator.GetLayerWeight(i) == 1f)
            .DefaultIfEmpty(-1)
            .First();

        protected override float PenisSize => 0.2f;

        protected override MethodInfo[] StartHMethods =>
            new[] { AccessTools.Method("SexSimControl, Assembly-CSharp:InitializeAsync") };

        protected override MethodInfo[] EndHMethods =>
            new[] { AccessTools.Method("SexSimControl, Assembly-CSharp:RunSexConclusion") };

        protected override Animator GetFemaleAnimator(int girlIndex) => naomiAnimator;

        protected override GameObject GetFemaleRoot(int girlIndex) => null;

        protected override Transform PenisBase =>
            GameObject.Find("cc_balls1.l")?.transform ?? transform;

        protected override string GetPose(int girlIndex) =>
            naomiAnimator.GetCurrentAnimatorClipInfo(AnimationLayer).FirstOrDefault().clip?.name
                ?? "unknown_pose";

        protected override bool IsIdle(int girlIndex) => !isSex.Value;

        protected override bool IsOrgasming(int girlIndex) => GetPose(0).Contains("Cum");

        protected override void SetStartHInstance(object sexSimControl) =>
            isSex = Traverse.Create(sexSimControl).Field<bool>("_sexActive");

        protected override IEnumerator UntilReady()
        {
            yield return new WaitWhile(() =>
                (naomiAnimator = GameObject.Find("NaomiRig").GetComponent<Animator>()) == null);
            animationLayers = Enumerable.Range(0, naomiAnimator.layerCount)
                .Where(i => layerNames.Contains(naomiAnimator.GetLayerName(i)));
        }
    }
}