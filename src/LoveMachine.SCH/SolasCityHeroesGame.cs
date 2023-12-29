using System.Collections;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.SCH;

internal class SolasCityHeroesGame : GameAdapter
{
    private const string EnemyWaist = "Enemy/ArmatureFem_000/Global/Position/waist";
    
    private Traverse sexSystem;
    
    protected override MethodInfo[] StartHMethods =>
        new[] { AccessTools.Method("SexSystem, Assembly-CSharp:StartSex") };
    
    protected override MethodInfo[] EndHMethods => new[]
    {
        AccessTools.Method("SexSystem, Assembly-CSharp:Escape"),
        AccessTools.Method("PlayerHub, Assembly-CSharp:Start")
    };

    protected override Dictionary<Bone, string> FemaleBoneNames => new()
    {
        { Bone.Vagina, "groin_R" },
        { Bone.Mouth, "MouthLower" },
        { Bone.LeftHand, "hand_L" },
        { Bone.RightHand, "hand_R" },
    };

    protected override Transform PenisBase => throw new NotImplementedException();

    protected override Transform[] PenisBases => new[]
    {
        GameObject.Find($"{EnemyWaist}/groin_R").transform,
        GameObject.Find($"{EnemyWaist}/stomach/torso/neck/head/MouthLower").transform,
        GameObject.Find($"{EnemyWaist}/stomach/torso/collar_L/shoulder_L/arm_L/hand_L").transform,
        GameObject.Find($"{EnemyWaist}/stomach/torso/collar_R/shoulder_R/arm_R/hand_R").transform
    };
    
    protected override int AnimationLayer => 0;
    
    protected override int HeroineCount => 1;
    
    protected override int MaxHeroineCount => 1;
    
    protected override bool IsHardSex => true;

    protected override Animator GetFemaleAnimator(int girlIndex) =>
        sexSystem.Property<Animator>("PlayerAnim").Value;
    
    protected override GameObject GetFemaleRoot(int girlIndex) => GameObject.Find("/Player");

    protected override string GetPose(int girlIndex) =>
        GetFemaleAnimator(0).GetCurrentAnimatorStateInfo(0).fullPathHash.ToString();

    protected override bool IsIdle(int girlIndex) => Time.timeScale == 0f;

    protected override IEnumerator UntilReady(object instance)
    {
        yield return new WaitForSecondsRealtime(1f);
        sexSystem = Traverse.Create(instance);
    }
}