using System.Reflection;
using HarmonyLib;
using LoveMachine.Core.Game;
using LoveMachine.Core.Common;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace LoveMachine.HKR;

internal class HolyKnightRiccaGame : TimelineGameAdapter
{
    private static readonly string[] dickBasePaths =
    {
        "DEF-testicle",
        "ORG-testicle",
        "DEF-Ovipositor",
        "slimeAtd/root/DEF-mainTentacle_030",
        "roper/base/mainTentacle03_013",
        "gazer/root/lowerHand_L_006",
        "hellplant_parasiteIvy/root/DEF-ivy_020",
        "hellbeetle/Base/belly/belly_001/belly_002/tail",
        "arclichTentacleSet/root_front_L/DEF-tentacle_front_L_012",
        "Sword01/zweihander",
        "magicDildo/base/dildoBase/ball_L",
        "ClaretParent/rcWG_noLoad_atd/RicassoModule/RicassoSlopeParent/rcc_maleGenital",
        "RicassoKnightActor_wing_noLoad_atd/RicassoModule/RicassoSlopeParent/rcc_maleGenital"
    };

    private PlayableDirector AtdAnvDirector => FindObjectOfType<PlayableDirector>();

    protected override MethodInfo[] StartHMethods => new[]
    {
        AccessTools.Method("ANV.NovelImageController, ANVAssemblyDifinition:Initialize"),
        AccessTools.Method("ATD.UIController, ATDAssemblyDifinition:FinishADV")
    };

    protected override MethodInfo[] EndHMethods => new[]
    {
        AccessTools.Method("ANV.NovelImageController, ANVAssemblyDifinition:OnDestroy"),
        AccessTools.Method("ATD.UIController, ATDAssemblyDifinition:OnDestroy")
    };
    
    protected override Traverse Director => Traverse.Create(AtdAnvDirector);

    protected override Traverse Timeline =>
        Traverse.Create(FindObjectOfType<PlayableDirector>().playableAsset.Cast<TimelineAsset>());
    
    protected override string TrackName => "Cut Track Asset";
    
    protected override Dictionary<Bone, string> FemaleBoneNames => new()
    {
        { Bone.Vagina, "DEF-clitoris" },
        { Bone.Mouth, "MouseTransform" }
    };

    protected override int HeroineCount => 1;
    protected override int MaxHeroineCount => 1;
    protected override bool IsHardSex => true;
    protected override float MinStrokeLength => 0.3f;
    protected override float PenisSize => 0.1f;
    protected override Transform PenisBase => throw new NotImplementedException();

    protected override Transform[] PenisBases => dickBasePaths
        .SelectMany(path => FindDeepChildrenByPath(AtdAnvDirector.gameObject, path))
        .ToArray();

    protected override GameObject GetFemaleRoot(int girlIndex) => GameObject.Find("ricasso/root");

    protected override bool IsIdle(int girlIndex) => false;
}