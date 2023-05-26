using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.OT
{
    public class OedoTriggerGame : GameDescriptor
    {
        private Traverse director;
        private Traverse timeline;
        private Dictionary<string, Traverse> clipCache;
        private TimeUnlooper unlooper;
        private string cachedPose;

        protected override MethodInfo[] StartHMethods =>
            new[] { AccessTools.Method("GalleryController, NKAssets:StartMotionPlay") };
        
        protected override MethodInfo[] EndHMethods =>
            new[] { AccessTools.Method("GalleryController, NKAssets:EndGallery") };

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "PssyPos" }
        };

        protected override Transform PenisBase => GameObject.Find(
            "SubSystem/NKDirection/PlayableDirector/BoyOriginal/rig/root/DEF-spine/Penis0")
            .transform;
        
        protected override int AnimationLayer => throw new NotImplementedException();
        
        protected override int HeroineCount => 1;
        
        protected override int MaxHeroineCount => 3;
        
        protected override bool IsHardSex => false;
        
        protected override Animator GetFemaleAnimator(int girlIndex) =>
            throw new NotImplementedException();
        
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
            double directorTime = director.Property<double>("time").Value;
            double clipStart = clip.Property<double>("start").Value;
            double clipDuration = clip.Property<double>("duration").Value;
            float time = (float)((directorTime - clipStart) / clipDuration);
            normalizedTime = unlooper.LoopingToMonotonic(time);
            length = (float)clipDuration;
            speed = 1f;
        }

        private Traverse GetCurrentClip() => Enumerable
            .Range(0, timeline.Property<int>("outputTrackCount").Value)
            .Select(i => timeline.Method("GetOutputTrack", i).GetValue())
            .Select(Traverse.Create)
            .Where(track => track.Property<string>("name").Value == "Loop Track")
            .SelectMany(track => track.Property<IEnumerable<object>>("clips").Value)
            .Select(Traverse.Create)
            .FirstOrDefault(clip =>
                clip.Property<double>("start").Value < director.Property<double>("time").Value &&
                director.Property<double>("time").Value < clip.Property<double>("end").Value);

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            GameObject.Find("SubSystem/NKDirection/PlayableDirector/CQC_GirlOriginal_G1/rig/root");

        protected override string GetPose(int girlIndex) =>
            cachedPose = GetCurrentClip()?.Property<double>("start").Value.ToString() ?? cachedPose ?? "unknown";

        protected override bool IsIdle(int girlIndex) => false;

        protected override IEnumerator UntilReady()
        {
            var directorType =
                Type.GetType("UnityEngine.Playables.PlayableDirector, UnityEngine.DirectorModule");
            object directorObj = null;
            while (directorObj == null)
            {
                yield return new WaitForSeconds(1f);
                directorObj = FindObjectOfType(directorType);
            }
            director = Traverse.Create(directorObj);
            timeline = director.Property("playableAsset");
            clipCache = new Dictionary<string, Traverse>();
            unlooper = new TimeUnlooper();
        }
    }
}