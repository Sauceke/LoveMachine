using HarmonyLib;
using LoveMachine.Core;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LoveMachine.LE
{
    public class LastEvilGame : GameDescriptor
    {
        private Animation animation;
        private Traverse<int> animIndex;

        public override int AnimationLayer => throw new System.NotImplementedException();

        protected override MethodInfo[] StartHMethods =>
            new[] { AccessTools.Method("EventSceneFramework, Assembly-CSharp:Init") };

        protected override MethodInfo[] EndHMethods =>
            new[] { AccessTools.Method("EventSceneFramework, Assembly-CSharp:OnClickEnd") };

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "Bip_Clitoris2" },
            { Bone.Anus, "Bip_Ass7_2" },
            { Bone.Mouth, "DUMMY_HEAD" },
            { Bone.LeftHand, "Bip_FingerL2_4" },
            { Bone.RightHand, "Bip_FingerR2_4" },
            { Bone.LeftBreast, "Bip_NippleL" },
            { Bone.RightBreast, "Bip_NippleR" }
        };

        protected override int HeroineCount => 1;

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => animIndex.Value > 1;

        public override Animator GetFemaleAnimator(int girlIndex) =>
            throw new System.NotImplementedException();

        protected override void GetAnimState(int girlIndex, out float normalizedTime,
            out float length, out float speed)
        {
            var state = animation[$"Anim{animIndex.Value + 1}"];
            normalizedTime = state.time / state.length;
            length = state.length;
            speed = 1f;
        }

        protected override Transform PenisBase => throw new System.NotImplementedException();

        protected override Transform[] PenisBases => FindDeepChildrenByName(
            GameObject.Find("EventSceneFramework/Root/Entities"), "ActorMan_Ball2");

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            GameObject.Find("EventSceneFramework/Root/Entities/Succubus");

        protected override string GetPose(int girlIndex) => animIndex.Value.ToString();

        protected override bool IsIdle(int girlIndex) => false;

        protected override void SetStartHInstance(object eventSceneFramework)
        {
            var traverse = Traverse.Create(eventSceneFramework);
            animation = traverse.Field<Animation>("_animation").Value;
            animIndex = traverse.Field<int>("_animIndex");
        }

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSeconds(5f);
        }
    }
}