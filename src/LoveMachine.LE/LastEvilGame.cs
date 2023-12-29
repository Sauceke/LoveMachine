using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Game;
using LoveMachine.Core.Common;
using UnityEngine;

namespace LoveMachine.LE
{
    public class LastEvilGame : GameAdapter
    {
        private const string root = "EventSceneFramework/Root/Entities";
        private static readonly string[] ballsNames =
        {
            "ActorMan_Ball2",
            "Dick_Ball2",
            "Succubus_Doppelganger/Bip_Root/Bip_Dick_1",
            "Bip_Ball",
            "Bip_Ball02",
            "Slime_Collect_Acid",
            "Slime_AnimEvent1/Bone001/Bone002/Bone003/Bone004/Bone005",
            "Slime_Collect/Bip01_Root/Bip01_Bone1/Bip01_Bone2/Bip01_Bone3/Bip01_Bone4/Bip01_Bone5",
            "Slime_Defeat/Bip01_Root/Bip01_Bone1/Bip01_Bone2/Bip01_Bone3/Bip01_Bone4",
            "TentacleSub (1)/Bip01/Bip02/Bip03/Bip04/Bip05",
            "TentacleSub (2)/Bip01/Bip02/Bip03/Bip04/Bip05",
            "TentacleSub_Virgin/Bip01/Bip02/Bip03/Bip04/Bip05",
            "TentacleSub_Mouth/Bip01/Bip02/Bip03/Bip04/Bip05",
            "TentacleSub_Ass/Bip01/Bip02/Bip03/Bip04/Bip05",
            "Mimic/Bip01_Box/Bip01_BoxBody/Bip01_Tentacle (3)/Bip01_Tentacle_02/" +
                "Bip01_Tentacle_03/Bip01_Tentacle_04/Bip01_Tentacle_05/Bip01_Tentacle_06/" +
                "Bip01_Tentacle_07/Bip01_Tentacle_08/Bip01_Tentacle_09/Bip01_Tentacle_10",
            "Mimic/Bip01_Box/Bip01_BoxBody/Bip01_Tentacle (5)/Bip01_Tentacle_02/"+
                "Bip01_Tentacle_03/Bip01_Tentacle_04/Bip01_Tentacle_05/Bip01_Tentacle_06/"+
                "Bip01_Tentacle_07/Bip01_Tentacle_08/Bip01_Tentacle_09/Bip01_Tentacle_10"
        };

        private Animation animation;
        private Traverse<int> animIndex;

        protected override int AnimationLayer => throw new NotImplementedException();

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

        protected override Animator GetFemaleAnimator(int girlIndex) =>
            throw new NotImplementedException();

        protected override void GetAnimState(int girlIndex, out float normalizedTime,
            out float length, out float speed)
        {
            var state = animation[$"Anim{animIndex.Value + 1}"];
            normalizedTime = state.time / state.length;
            length = state.length;
            speed = 1f;
        }

        protected override Transform PenisBase => throw new NotImplementedException();

        protected override Transform[] PenisBases => ballsNames
            .SelectMany(path => FindDeepChildrenByPath(GameObject.Find(root), path))
            .ToArray();

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            GameObject.Find(root + "/Succubus");

        protected override string GetPose(int girlIndex) => animIndex.Value.ToString();

        protected override bool IsIdle(int girlIndex) => false;
        
        protected override IEnumerator UntilReady(object eventSceneFramework)
        {
            yield return new WaitForSeconds(5f);
            var traverse = Traverse.Create(eventSceneFramework);
            animation = traverse.Field<Animation>("_animation").Value;
            animIndex = traverse.Field<int>("_animIndex");
        }
    }
}