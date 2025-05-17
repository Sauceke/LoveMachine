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
            // Type 0: From Gallery
            //[Info: LoveMachine] UntilReady: Anim_04_End
            //[Info: LoveMachine] UntilReady: Anim_03_Emit
            //[Info: LoveMachine] UntilReady: Anim_02_During
            //[Info: LoveMachine] UntilReady: Anim_01_Start
            //[Info: LoveMachine] UntilReady: Animation index: 0

            // Type 1: In Game
            //[Info: LoveMachine] UntilReady: Anim4
            //[Info: LoveMachine] UntilReady: Anim3
            //[Info: LoveMachine] UntilReady: Anim2
            //[Info: LoveMachine] UntilReady: Anim1
            //[Info: LoveMachine] UntilReady: Animation index: 0

            //Logger.LogInfo($"GetAnimState, girl{girlIndex}");
            //Logger.LogInfo($"GetAnimState: Animation index: {animIndex.Value}");
            //// List of name of all anime states
            //foreach (var anim in animation)
            //{
            //    // Force convert to AnimationState
            //    // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            //    Logger.LogInfo("GetAnimState:" + ((AnimationState)anim).name);
            //}

            AnimationState state = null;
            bool animationFound = false;
            int index = animIndex.Value + 1;

            // Try Type 1 format first (e.g., "Anim1")
            string type1Name = $"Anim{index}";
            try
            {
                state = animation[type1Name];
                animationFound = true;
            }
            catch
            {
                // Type 1 format failed, now try Type 0 format
            }

            // If Type 1 failed, try Type 0 format (e.g., "Anim_01_Start")
            if (!animationFound)
            {
                string[] suffixes = { "_Start", "_During", "_Emit", "_End" };
                string formattedIndex = index.ToString("00");

                foreach (string suffix in suffixes)
                {
                    string type0Name = $"Anim_{formattedIndex}{suffix}";
                    try
                    {
                        state = animation[type0Name];
                        animationFound = true;
                        break;
                    }
                    catch
                    {
                        // This particular suffix didn't work, try the next one
                    }
                }
            }

            // Set output values
            if (animationFound && state != null)
            {
                normalizedTime = state.time / state.length;
                length = state.length;
                speed = 1f;
            }
            else
            {
                // No valid animation found, set default values
                normalizedTime = 0f;
                length = 0f;
                speed = 1f;
            }
        }

        protected override Transform PenisBase => throw new NotImplementedException();

        protected override Transform[] PenisBases => ballsNames
            .SelectMany(path => FindDeepChildrenByPath(GameObject.Find(root), path))
            .ToArray();

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            GameObject.Find(root + "/Succubus");

        protected override string GetPose(int girlIndex) => animIndex.Value.ToString();

        protected override bool IsIdle(int girlIndex) => false;

        protected override bool IsOrgasming(int girlIndex) => animIndex.Value==2;

        protected override IEnumerator UntilReady(object eventSceneFramework)
        {
            yield return new WaitForSeconds(5f);
            var traverse = Traverse.Create(eventSceneFramework);
            animation = traverse.Field<Animation>("_animation").Value;
            animIndex = traverse.Field<int>("_animIndex");
        }
    }
}