using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.HS
{
    internal sealed class HoneySelectGame : GameAdapter
    {
        private static readonly string[] idleAnimations =
        {
            "Idle", "WIdle", "SIdle", "Insert", "D_Idle", "D_Insert",
            "Orgasm_A", "Orgasm_OUT_A", "Drink_A", "Vomit_A", "Choke_A",
            "Orgasm_IN_A", "OrgasmM_OUT_A", "D_Orgasm_A", "D_Orgasm_OUT_A",
            "D_Drink_A", "D_Vomit_A", "D_Choke_A", "D_Orgasm_IN_A", "D_OrgasmM_OUT_A"
        };
        
        private static readonly string[] orgasmAnimations =
        {
            "D_OrgasmM_IN", "D_OrgasmM_OUT", "D_OrgasmM", "D_OrgasmF_IN", "D_OrgasmF_OUT",
            "D_OrgasmF", "D_OrgasmS_IN", "D_OrgasmS_OUT", "D_OrgasmS", "D_Orgasm_IN",
            "D_Orgasm_OUT", "D_Orgasm", "OrgasmM_IN", "OrgasmM_OUT", "OrgasmM", "OrgasmF_IN",
            "OrgasmF_OUT", "OrgasmF", "OrgasmS_IN", "OrgasmS_OUT", "OrgasmS", "Orgasm_IN",
            "Orgasm_OUT", "Orgasm"
        };
        
        private HScene scene;
        private GameObject female;
        private Animator femaleAnimator;
        
        protected override MethodInfo[] StartHMethods =>
            new[] { AccessTools.Method(typeof(HScene), "SetStartVoice") };
        
        protected override MethodInfo[] EndHMethods =>
            new[] { AccessTools.Method(typeof(HScene), "OnDestroy") };
        
        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "cf_J_Kokan" },
            { Bone.RightHand, "cf_J_Hand_Wrist_s_R" },
            { Bone.LeftHand, "cf_J_Hand_Wrist_s_L" },
            { Bone.RightBreast, "cf_J_Mune04_s_R" },
            { Bone.LeftBreast, "cf_J_Mune04_s_L" },
            { Bone.Mouth, "cf_J_MouthCavity" },
            { Bone.RightFoot, "cf_J_Toes01_L" },
            { Bone.LeftFoot, "cf_J_Toes01_R" }
        };
        protected override Transform PenisBase => GameObject.Find("cm_J_dan_f_L").transform;
        protected override int AnimationLayer => 0;
        protected override int HeroineCount => 1;
        protected override int MaxHeroineCount => 1;
        protected override bool IsHardSex => false;

        protected override Animator GetFemaleAnimator(int girlIndex) => femaleAnimator;

        protected override GameObject GetFemaleRoot(int girlIndex) => female;

        protected override string GetPose(int girlIndex) =>
            scene.ctrlFlag.nowAnimationInfo.id
                + "." + GetAnimatorStateInfo(girlIndex).fullPathHash;

        protected override bool IsIdle(int girlIndex) => 
            idleAnimations.Any(GetAnimatorStateInfo(girlIndex).IsName);

        protected override bool IsOrgasming(int girlIndex) => 
            orgasmAnimations.Any(GetAnimatorStateInfo(girlIndex).IsName);

        protected override IEnumerator UntilReady(object instance)
        {
            yield return new WaitForSeconds(5f);
            scene = (HScene)instance;
            female = GameObject.Find("/CommonSpace/chaF00");
            femaleAnimator = GameObject.Find("/CommonSpace/chaF00/BodyTop/p_cf_anim")
                .GetComponent<Animator>();
        }
    }
}