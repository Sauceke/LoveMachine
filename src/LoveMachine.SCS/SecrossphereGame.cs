using HarmonyLib;
using LoveMachine.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoveMachine.SCS
{
    internal class SecrossphereGame : GameDescriptor
    {
        private MonoBehaviour scene;
        private MonoBehaviour[] females;
        private Animator[] femaleAnimators;
        private Traverse<int> state;

        public override int AnimationLayer => 0;

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "a01_J_NikuR_00" },
            { Bone.Mouth, "A00_J_sita_02" },
            { Bone.LeftHand, "a01_J_HitoL_03" },
            { Bone.RightHand, "a01_J_HitoR_03" }
        };

        protected override int HeroineCount => females.Length;

        protected override int MaxHeroineCount => 2; // no idea tbh

        protected override bool IsHardSex => false;

        protected override float PenisSize => 0.15f;

        public override Animator GetFemaleAnimator(int girlIndex) => femaleAnimators[girlIndex];

        protected override Transform GetDickBase() => GameObject.Find("a_J_tamaL").transform;

        protected override GameObject GetFemaleRoot(int girlIndex) => females[girlIndex].gameObject;

        protected override string GetPose(int girlIndex) =>
            GetFemaleAnimator(girlIndex).GetCurrentAnimatorStateInfo(0).fullPathHash.ToString();

        protected override bool IsIdle(int girlIndex) => state.Value < 0 || state.Value > 2;

        protected override bool IsOrgasming(int girlIndex) => state.Value == 3;

        public void StartH(MonoBehaviour scene)
        {
            this.scene = scene;
            StartH();
        }

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSeconds(5f);
            females = Traverse.Create(scene)
                .Property<IList>("Humans").Value
                .Cast<object>()
                .Select(Traverse.Create)
                .Select(chara => chara.Field("human"))
                .Where(chara => chara.Property<int>("CharaID").Value != 0)
                .Select(chara => chara.GetValue<MonoBehaviour>())
                .ToArray();
            femaleAnimators = females.Select(Traverse.Create)
                .Select(female => female.Property<Animator>("Anime").Value)
                .ToArray();
            state = Traverse.Create(scene).Property<int>("NowState");
        }
    }
}