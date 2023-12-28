using System.Collections;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.AIDR;

public class AIDealRaysGame : GameAdapter
{
    private GameObject AIdeal;
    private Animator AIdealAnimator;

    protected override MethodInfo[] StartHMethods =>
        new[] { AccessTools.Method("EventManager, Assembly-CSharp:ActiveHEvent") };
    
    protected override MethodInfo[] EndHMethods =>
        new[] { AccessTools.Method("EventManager, Assembly-CSharp:EventFinish") };

    protected override Dictionary<Bone, string> FemaleBoneNames => new()
    {
        { Bone.Vagina, "Hips.vag" },
        { Bone.Mouth, "Chin" },
        { Bone.LeftHand, "Index Distal.L_end" },
        { Bone.RightHand, "Index Distal.R_end" },
        { Bone.LeftFoot, "Toe.L_end" },
        { Bone.RightFoot, "Toe.R_end" },
        { Bone.LeftBreast, "Oppai_end.L_end" },
        { Bone.RightBreast, "Oppai_end.R_end" }
    };

    protected override Transform PenisBase => GameObject.Find("tamaL01").transform;
    protected override int AnimationLayer => 0;
    protected override int HeroineCount => 1;
    protected override int MaxHeroineCount => 1;
    protected override bool IsHardSex => false;

    protected override Animator GetFemaleAnimator(int girlIndex) => AIdealAnimator;

    protected override GameObject GetFemaleRoot(int girlIndex) => AIdeal;

    protected override string GetPose(int girlIndex) =>
        GetAnimatorStateInfo(0).fullPathHash.ToString();

    protected override bool IsIdle(int girlIndex) => false;

    protected override void OnStartH(object instance)
    {
        var eventManager = Traverse.Create(instance);
        AIdeal = eventManager.Property<GameObject>("AIdeal").Value;
        AIdealAnimator = eventManager.Property<Animator>("AIdealAnimator").Value;
    }

    protected override IEnumerator UntilReady()
    {
        yield return new WaitForSeconds(1f);
    }
}