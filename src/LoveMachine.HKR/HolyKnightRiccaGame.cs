using HarmonyLib;
using LoveMachine.Core;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace LoveMachine.HKR;

internal class HolyKnightRiccaGame : GameDescriptor
{
    private static readonly string[] dickBasePaths =
    {
        "DEF-testicle",
        "ORG-testicle",
        "DEF-Ovipositor",
        "hellbeetle/Base/belly/belly_001/belly_002/tail"
    };

    private PlayableDirector director;
    private TimelineAsset timeline;
    private Dictionary<string, TimelineClip> clipCache;
    private TimeUnlooper unlooper;
    private string cachedPose;

    protected override int AnimationLayer => 0;

    protected override Dictionary<Bone, string> FemaleBoneNames => new()
    {
        { Bone.Vagina, "DEF-clitoris" },
        { Bone.Mouth, "MouseTransform" }
    };

    protected override int HeroineCount => 1;

    protected override int MaxHeroineCount => 1;

    protected override bool IsHardSex => true;

    protected override float PenisSize => 0.1f;

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

    protected override Animator GetFemaleAnimator(int girlIndex) =>
        throw new NotImplementedException();

    protected override Transform PenisBase => dickBasePaths
        .Select(GameObject.Find)
        .First(go => go != null)
        .transform;

    protected override GameObject GetFemaleRoot(int girlIndex) =>
        GameObject.Find("ricasso/root");

    protected override string GetPose(int girlIndex) =>
        cachedPose = GetCurrentClip()?.displayName ?? cachedPose;

    protected override bool IsIdle(int girlIndex) => false;

    protected override void GetAnimState(int girlIndex, out float normalizedTime,
        out float length, out float speed)
    {
        string pose = GetPose(0);
        if (!clipCache.TryGetValue(pose, out var clip))
        {
            clip = GetCurrentClip();
            if (clip == null)
            {
                normalizedTime = 0f;
                length = 1f;
                speed = 1f;
                return;
            }
            clipCache[pose] = clip;
        }
        float time = (float)((director.time - clip.start) / clip.duration);
        normalizedTime = unlooper.LoopingToMonotonic(time);
        length = (float)clip.duration;
        speed = 1f;
    }

    private TimelineClip? GetCurrentClip() => Enumerable
        .Range(0, timeline.outputTrackCount)
        .Select(timeline.GetOutputTrack)
        .Where(track => director.GetGenericBinding(track)?.name == director.name)
        .Where(track => track.name == "Cut Track Asset")
        .SelectMany(track => track.clips)
        .FirstOrDefault(clip => clip.start < director.time && director.time < clip.end);

    protected override IEnumerator UntilReady()
    {
        while (director == null)
        {
            yield return new WaitForSeconds(1f);
            director = FindObjectOfType<PlayableDirector>();
        }
        timeline = director.playableAsset.Cast<TimelineAsset>();
        clipCache = new Dictionary<string, TimelineClip>();
        unlooper = new TimeUnlooper();
    }
}