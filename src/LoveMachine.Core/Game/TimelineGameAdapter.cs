using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using LoveMachine.Core.Util;
using UnityEngine;

namespace LoveMachine.Core.Game
{
    /// <summary>
    /// GameAdapter for games that use Unity Timeline to animate H-scenes.
    /// </summary>
    public abstract class TimelineGameAdapter : GameAdapter
    {
        private Traverse director;
        private Traverse timeline;
        private Dictionary<string, Traverse> clipCache;
        private TimeUnlooper unlooper;
        private string cachedPose;

        /// <summary>
        /// Traverse of the PlayableDirector that manages H-scene animations.
        /// </summary>
        [HideFromIl2Cpp]
        protected abstract Traverse Director { get; }
        
        /// <summary>
        /// Traverse of the TimelineAsset that contains the H-scene animations.
        /// </summary>
        [HideFromIl2Cpp]
        protected abstract Traverse Timeline { get; }
        
        /// <summary>
        /// Name of the Timeline track that contains the H-scene animations.
        /// </summary>
        [HideFromIl2Cpp]
        protected abstract string TrackName { get; }
        
        protected override int AnimationLayer => throw new NotImplementedException();
        
        protected override Animator GetFemaleAnimator(int girlIndex) =>
            throw new NotImplementedException();
        
        protected internal override void GetAnimState(int girlIndex, out float normalizedTime,
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
            .Where(track => track.Property<string>("name").Value == TrackName)
            .SelectMany(track => track.Property<IEnumerable<object>>("clips").Value)
            .Select(Traverse.Create)
            .FirstOrDefault(clip =>
                clip.Property<double>("start").Value < director.Property<double>("time").Value &&
                director.Property<double>("time").Value < clip.Property<double>("end").Value);

        protected internal override string GetPose(int girlIndex)
        {
            var clip = GetCurrentClip();
            if (clip == null)
            {
                return cachedPose ?? "unknown_pose";
            }
            return cachedPose = clip.Property<string>("displayName").Value + "." +
                clip.Property<double>("start").Value;
        }

        protected override IEnumerator UntilReady()
        {
            while (Director.GetValue() == null)
            {
                yield return new WaitForSeconds(1f);
            }
            director = Director;
            timeline = Timeline;
            clipCache = new Dictionary<string, Traverse>();
            cachedPose = null;
            unlooper = new TimeUnlooper();
        }
    }
}