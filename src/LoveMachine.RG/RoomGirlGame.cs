using System.Collections;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Game;
using LoveMachine.Core.Common;
using UnityEngine;

namespace LoveMachine.RG;

internal class RoomGirlGame : GameAdapter
{
    private Animator femaleAnimator;
    private Traverse ctrlFlag;
    private Traverse<int> loopType;
    private Traverse<bool> nowOrgasm;

    private Traverse AnimationInfo => ctrlFlag.Property("NowAnimationInfo");

    private int AnimationId => AnimationInfo.Property<int>("ID").Value;

    private string AnimationName => AnimationInfo.Property<string>("NameAnimation").Value;

    protected override int AnimationLayer => 0;

    protected override Dictionary<Bone, string> FemaleBoneNames => new()
    {
        { Bone.Vagina, "cf_J_Kokan" },
        { Bone.RightHand, "cf_J_Hand_Index01_R" },
        { Bone.LeftHand, "cf_J_Hand_Index01_L" },
        { Bone.RightBreast, "cf_J_Mune_Nip01_R" },
        { Bone.LeftBreast, "cf_J_Mune_Nip01_L" },
        { Bone.Mouth, "cf_J_MouthCavity" },
        { Bone.RightFoot, "cf_J_Toes01_L" },
        { Bone.LeftFoot, "cf_J_Toes01_R" }
    };

    protected override int HeroineCount => 1;

    protected override int MaxHeroineCount => 1;

    protected override bool IsHardSex => false;

    protected override float PenisSize => 0.8f;

    protected override MethodInfo[] StartHMethods =>
        new[] { AccessTools.Method("HScene, Assembly-CSharp:Start") };

    protected override MethodInfo[] EndHMethods =>
        new[] { AccessTools.Method("HScene, Assembly-CSharp:OnDestroy") };

    protected override Animator GetFemaleAnimator(int girlIndex) => femaleAnimator;

    protected override Transform PenisBase => GameObject.Find(
        "chaM_001/BodyTop/p_cf_anim/cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Kosi01/cf_J_Kosi02/" +
        "cm_J_dan_s/cm_J_dan_top/cm_J_dan_f_top/k_m_tamaC_00").transform;

    protected override GameObject GetFemaleRoot(int girlIndex) => GameObject.Find("chaF_001");

    protected override string GetPose(int girlIndex) =>
        $"{AnimationName}.{AnimationId}.{GetAnimatorStateInfo(girlIndex).fullPathHash}";

    protected override bool IsIdle(int girlIndex) => loopType.Value == -1;

    protected override bool IsOrgasming(int girlIndex) => nowOrgasm.Value;
    
    protected override IEnumerator UntilReady(object hscene)
    {
        while (GetFemaleRoot(0) == null)
        {
            yield return new WaitForSeconds(5f);
        }
        ctrlFlag = Traverse.Create(hscene).Property("CtrlFlag");
        loopType = ctrlFlag.Property<int>("LoopType");
        nowOrgasm = ctrlFlag.Property<bool>("NowOrgasm");
        femaleAnimator = GameObject.Find("chaF_001/BodyTop/p_cf_anim").GetComponent<Animator>();
    }
}