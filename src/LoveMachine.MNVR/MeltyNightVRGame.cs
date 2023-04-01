using System.Collections;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.MNVR;

public class MeltyNightVRGame : GameDescriptor
{
    private Traverse<Animator> animator;
    private Traverse<GameObject> tnp;
    private Traverse<string> h_motion;
    private Traverse<bool> Free;
    private Traverse body;

    protected override MethodInfo[] StartHMethods =>
        new[] { AccessTools.Method("SexController, Assembly-CSharp:SexStart") };

    protected override MethodInfo[] EndHMethods => new MethodInfo[] { };

    protected override Dictionary<Bone, string> FemaleBoneNames =>
        throw new NotImplementedException();

    protected override Dictionary<Bone, Transform> GetFemaleBones(int girlIndex) => new()
    {
        { Bone.Vagina, body.Property<GameObject>("Hips").Value.transform },
        { Bone.LeftHand, body.Property<GameObject>("Hand_L").Value.transform },
        { Bone.RightHand, body.Property<GameObject>("Hand_R").Value.transform }
    };

    protected override Transform PenisBase => tnp.Value?.transform;

    protected override int AnimationLayer => 0;
    protected override int HeroineCount => 1;
    protected override int MaxHeroineCount => 1;
    protected override bool IsHardSex => false;

    protected override Animator GetFemaleAnimator(int girlIndex) => animator.Value;

    protected override GameObject GetFemaleRoot(int girlIndex) =>
        throw new NotImplementedException();

    protected override string GetPose(int girlIndex) =>
        $"{h_motion.Value}.{PenisBase.position.x}.{PenisBase.position.y}.{PenisBase.position.z}";

    protected override bool IsIdle(int girlIndex) =>
        Free.Value || h_motion.Value.Contains("Idle");

    protected override void SetStartHInstance(object instance)
    {
        var sexControl = Traverse.Create(instance);
        var character = sexControl.Property("Character");
        animator = sexControl.Property<Animator>("animator");
        h_motion = character.Property<string>("h_motion");
        tnp = sexControl.Property<GameObject>("tnp");
        Free = sexControl.Property<bool>("Free");
        body = sexControl.Property("characterBody");
    }
    
    protected override IEnumerator UntilReady()
    {
        while (PenisBase == null)
        {
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
}