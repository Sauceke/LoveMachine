using System.Collections;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.HC.DigitalCraft;

public class DigitalCraftGame : GameAdapter
{
    private GameObject[] females;
    private Transform[] penises;
    
    protected override MethodInfo[] StartHMethods =>
        new[] { AccessTools.Method("DigitalCraft.DigitalCraft,Assembly-CSharp:InitSceneAsync") };
    
    protected override MethodInfo[] EndHMethods => new MethodInfo[] { };
    
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
    protected override int MaxHeroineCount => 4;
    protected override bool IsHardSex => false;

    protected override Animator GetFemaleAnimator(int girlIndex) =>
        females[girlIndex].GetComponent<Animator>();

    protected override GameObject GetFemaleRoot(int girlIndex) => females[girlIndex];

    protected override string GetPose(int girlIndex) => "scene";

    protected override bool IsIdle(int girlIndex) => females.Length == 0 || penises.Length == 0;

    protected override IEnumerator UntilReady(object instance)
    {
        yield return new WaitForSeconds(5f);
        var humanType = Il2CppType.From(Type.GetType("Character.HumanComponent,Assembly-CSharp"));
        var humans = FindObjectsOfType(humanType, false)
            .Select(human => human.Cast<MonoBehaviour>().transform)
            .ToArray();
        females = humans
            .Where(tf => tf.name.StartsWith("chaF_"))
            .Select(chara => (chara.Find("BodyTop/p_cf_jm_body_bone_00")
                ?? chara.Find("BodyTop/p_cf_jm_body_bone_00(Clone)")).gameObject)
            .ToArray();
        penises = humans
            .Where(tf => tf.name.StartsWith("chaM_"))
            .Select(chara => FindDeepChildrenByPath(chara.gameObject, "k_f_tamaC_00").First())
            .ToArray();
    }
}