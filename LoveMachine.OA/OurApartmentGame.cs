using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.OA
{
    internal sealed class OurApartmentGame : GameDescriptor
    {
        private static readonly string[] layerNames =
        {
            "Base SexSim", "From Behind SexSim", "Couch Missionary SexSim"
        };

        private Traverse<bool> isSex;
        private Animator naomiAnimator;
        
        public void OnStartH(MonoBehaviour manager)
        {
            var managerTraverse = Traverse.Create(manager);
            isSex = managerTraverse.Field<bool>("_sexActive");
            StartH();
        }

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

        public override int AnimationLayer => Enumerable.Range(0, naomiAnimator.layerCount)
            .Where(i => layerNames.Contains(naomiAnimator.GetLayerName(i))
                && naomiAnimator.GetLayerWeight(i) == 1f)
            .DefaultIfEmpty(-1)
            .First();

        protected override bool IsHSceneInterrupted => false;

        protected override float PenisSize => 0.2f;

        public override Animator GetFemaleAnimator(int girlIndex) => naomiAnimator;

        protected override GameObject GetFemaleRoot(int girlIndex) => null;

        protected override Transform GetDickBase() =>
            GameObject.Find("cc_balls1.l")?.transform ?? transform;

        protected override string GetPose(int girlIndex) =>
            AnimationLayer < 0
                ? "unknown_pose"
                : naomiAnimator.GetCurrentAnimatorClipInfo(AnimationLayer)[0].clip.name;

        protected override bool IsIdle(int girlIndex) => !isSex.Value;

        protected override IEnumerator UntilReady()
        {
            yield return new WaitWhile(() =>
                (naomiAnimator = GameObject.Find("NaomiRig").GetComponent<Animator>()) == null);
        }
    }
}
