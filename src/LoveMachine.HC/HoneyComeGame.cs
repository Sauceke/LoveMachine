using System.Collections;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.HC;

public class HoneyComeGame: GameAdapter
{
    private Traverse ctrlFlag;
    private GameObject[] females;
    private Transform[] penises;
    private Traverse<int> loopType;
    private Traverse<bool> nowOrgasm;
    
    private Traverse AnimationInfo => ctrlFlag.Property("NowAnimationInfo");

    private int AnimationId => AnimationInfo.Property<int>("ID").Value;

    private string AnimationName => AnimationInfo.Property<string>("NameAnimation").Value;
    
    protected override MethodInfo[] StartHMethods =>
        new[] { AccessTools.Method("H.HScene, Assembly-CSharp:Start") };

    protected override MethodInfo[] EndHMethods =>
        new[] { AccessTools.Method("H.HScene, Assembly-CSharp:OnDestroy") };

    protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
    {
        { Bone.LeftBreast, "k_f_munenipL_00" },
        { Bone.RightBreast, "k_f_munenipR_00" },
        { Bone.Vagina, "cf_n_pee" },
        { Bone.Anus, "k_f_ana_00" },
        { Bone.LeftButt, "k_f_siriL_00" },
        { Bone.RightButt, "k_f_siriR_00" },
        { Bone.Mouth, "cf_J_MouthCavity" },
        { Bone.LeftHand, "cf_j_index03_L" },
        { Bone.RightHand, "cf_j_index03_R" },
        { Bone.LeftFoot, "k_f_toeL_00" },
        { Bone.RightFoot, "k_f_toeR_00" },
    };

    protected override Transform PenisBase => throw new NotImplementedException();
    protected override Transform[] PenisBases => penises;
    protected override float PenisSize => 0.07f;
    protected override int AnimationLayer => 0;
    protected override int HeroineCount => females.Length;
    protected override int MaxHeroineCount => 2;
    protected override bool IsHardSex => false;

    protected override Animator GetFemaleAnimator(int girlIndex) =>
        females[girlIndex].GetComponent<Animator>();

    protected override GameObject GetFemaleRoot(int girlIndex) => females[girlIndex];

    protected override string GetPose(int girlIndex) =>
        $"{AnimationName}.{AnimationId}.{GetAnimatorStateInfo(girlIndex).fullPathHash}";
    
    protected override bool IsIdle(int girlIndex) => loopType.Value == -1;

    protected override bool IsOrgasming(int girlIndex) => nowOrgasm.Value;

    protected override void OnStartH(object hscene)
    {
        ctrlFlag = Traverse.Create(hscene).Property("CtrlFlag");
        loopType = ctrlFlag.Property<int>("LoopType");
        nowOrgasm = ctrlFlag.Property<bool>("NowOrgasm");
    }

    protected override IEnumerator UntilReady()
    {
        yield return new WaitForSeconds(10f);
        females = new[] { "chaF_00", "chaF_01" }
            .Select(GameObject.Find)
            .Where(go => go != null && go.active)
            .Select(chara => chara.transform.Find("BodyTop/p_cf_jm_body_bone_00").gameObject)
            .ToArray();
        penises = new[] { "chaM_00", "chaM_01" }
            .Select(GameObject.Find)
            .Where(go => go != null && go.active)
            .Select(chara => FindDeepChildrenByName(chara, "k_f_tamaC_00").First())
            .ToArray();
    }
}