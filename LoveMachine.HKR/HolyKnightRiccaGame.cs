using System.Collections;
using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.HKR
{
    internal class HolyKnightRiccaGame : GameDescriptor
    {
        private Traverse<string> cutName;

        public void StartH(MonoBehaviour uiController)
        {
            cutName = Traverse.Create(uiController)
                .Property("actorController")
                .Property<string>("currentCutName");
            StartH();
        }

        public override int AnimationLayer => 0;

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "DEF-clitoris" },
            { Bone.Mouth, "MouseTransform" },
            { Bone.RightHand, "DEF-f_index_01_R" },
            { Bone.LeftHand, "DEF-f_index_01_L" },
            { Bone.RightBreast, "DEF-nipple_R" },
            { Bone.LeftBreast, "DEF-nipple_L" }
        };

        protected override int HeroineCount => 1;

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => true;

        protected override bool IsHSceneInterrupted => false;

        public override Animator GetFemaleAnimator(int girlIndex) =>
            GameObject.Find("RicassoSlopeParent").GetComponent<Animator>();

        protected override Transform GetDickBase() => GameObject.Find("DEF-testicle").transform;

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            GameObject.Find("ricasso/root");

        protected override string GetPose(int girlIndex) => cutName.Value;

        protected override bool IsIdle(int girlIndex) => false;

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSeconds(5f);
        }
    }
}
