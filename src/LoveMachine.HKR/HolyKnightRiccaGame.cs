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
    private GameObject SceneRoot => GameObject.Find("ATDTimeline");
    
    private static readonly string[] dickBasePaths =
    {
        "DEF-testicle",
        "ORG-testicle",
        "DEF-Ovipositor",
        "mainTentacle03_013",
        "hellbeetle/Base/belly/belly_001/belly_002/tail"
    };

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
    
    protected override Traverse Director => Traverse.Create(FindObjectOfType<PlayableDirector>());

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
    protected override float PenisSize => 0.1f;
    protected override Transform PenisBase => throw new NotImplementedException();

    protected override Transform[] PenisBases => dickBasePaths
        .SelectMany(path => FindDeepChildrenByPath(SceneRoot, path))
        .ToArray();

    protected override GameObject GetFemaleRoot(int girlIndex) => GameObject.Find("ricasso/root");

    protected override bool IsIdle(int girlIndex) => false;
}