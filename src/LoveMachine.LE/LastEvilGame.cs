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
        private string root = "EventSceneFramework/Root/Entities";

        private static readonly string[] ballsNames =
        {
            "ActorMan_Ball2",
            "Dick_Ball2",
            "Succubus_Doppelganger/Bip_Root/Bip_Dick_1",
            "Bip_Ball",
            "Bip_Ball02",
            "Slime_Collect_Acid",
            "BipDealdoTwin1_03",
            "HumanGirl_Merchant/Bip_Root/Bip_Virgin/Bip_Clitoris1/Bip_Clitoris2",
            "Succubus/Bip_Root/Bip_Spine1/Bip_Spine2/Bip_Spine3/Bip_Spine4/Bip_ShoulderR/Bip_ArmR1/Bip_ArmR2/Bip_HandR/Bip_FingerR3_1/Bip_FingerR3_2/Bip_FingerR3_3/Bip_FingerR3_4",
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

        private static readonly string[] orgasming_names = {
            "Anim_03_Emit",
            "Anim3"
            };

        private Animation animation;
        private Traverse<int> animIndex;

        protected override int AnimationLayer => throw new NotImplementedException();

        protected void myEndH() { }
        protected override MethodInfo[] StartHMethods =>
            new[] { AccessTools.Method("EventSceneFramework, Assembly-CSharp:Init"),
                AccessTools.Method("AnimCombineEventer, Assembly-CSharp:Set")
            };

        protected override MethodInfo[] EndHMethods =>
            new[] { AccessTools.Method("EventSceneFramework, Assembly-CSharp:OnClickEnd"),
            AccessTools.Method(typeof(LastEvilGame), nameof(myEndH))};

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
            if (animation == null)
            {
                Logger.LogError("GetAnimState: Animation is null, endH");
                normalizedTime = 0f;
                length = 0f;
                speed = 1f;

                myEndH();
                return;
            }

            AnimationState state = getPlayingAnim();

            // Set output values
            if (state != null)
            {   
                normalizedTime = state.time / state.length;
                length = state.length;
                speed = state.speed;

                //Logger.LogInfo("GetAnimState: Animation state found: " + state.name +
                //    " normalizedTime: " + normalizedTime +
                //    " length: " + length +
                //    " speed: " + speed);
            }
            else
            {
                foreach (var anim in animation)
                {
                    Logger.LogInfo("GetAnimState Not Hit:" + ((AnimationState)anim).name);
                }
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

        protected override bool IsOrgasming(int girlIndex)
            {
            AnimationState state = getPlayingAnim();
            if(state == null)
            {
                return false;
            }else
            {
                return orgasming_names.Contains(state.name);
            }
        }

        protected AnimationState getPlayingAnim()
        {
            if(animation==null)
            {
                return null;
            }

            foreach (AnimationState anim in animation)
            {
                if (animation.IsPlaying(anim.name))
                {
                    return anim;
                }
            }
            return null;
        }
        protected override IEnumerator UntilReady(object eventSceneFramework)
        {
            yield return new WaitForSeconds(5f);
            var traverse = Traverse.Create(eventSceneFramework);
            animation = traverse.Field<Animation>("_animation").Value;
            animIndex = traverse.Field<int>("_animIndex");

            // Try to cast eventSceneFramework to the correct type, like Unity.GameObject
            UnityEngine.Object obj = eventSceneFramework as UnityEngine.Object;
            root = obj.name;
        }
    }
}