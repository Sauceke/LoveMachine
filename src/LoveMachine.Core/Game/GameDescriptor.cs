using Il2CppInterop.Runtime.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LoveMachine.Core
{
    /// <summary>
    /// Contains everything LoveMachine needs to interface with a game.
    /// </summary>
    public abstract class GameDescriptor : CoroutineHandler
    {
        internal event EventHandler<HEventArgs> OnHStarted;

        internal event EventHandler<HEventArgs> OnHEnded;

        /// <summary>
        /// The H-scene start method(s) in the game assembly.
        /// </summary>
        [HideFromIl2Cpp]
        protected internal abstract MethodInfo[] StartHMethods { get; }

        /// <summary>
        /// The H-scene end method(s) in the game assembly.
        /// </summary>
        [HideFromIl2Cpp]
        protected internal abstract MethodInfo[] EndHMethods { get; }

        /// <summary>
        /// The name/path of each bone's GameObject in female characters. <br/>
        /// Not all bones are required to have an entry. <br/>
        /// Do NOT add an entry for Bone.Auto!
        /// </summary>
        [HideFromIl2Cpp]
        protected internal abstract Dictionary<Bone, string> FemaleBoneNames { get; }

        /// <summary>
        /// The base of the penis or something close to that (e.g. balls). <br/>
        /// If there are multiple penises, override PenisBases instead.
        /// </summary>
        [HideFromIl2Cpp]
        protected abstract Transform PenisBase { get; }

        /// <summary>
        /// The layer index of the sex animation in female animators
        /// (if the game uses animators).
        /// </summary>
        [HideFromIl2Cpp]
        protected abstract int AnimationLayer { get; }

        /// <summary>
        /// Number of heroines in the current H-scene. <br/>
        /// This is NOT expected to change during an H-scene! Any code path that
        /// changes it must be covered by StartHMethods.
        /// </summary>
        [HideFromIl2Cpp]
        protected internal abstract int HeroineCount { get; }

        /// <summary>
        /// The maximum possible number of heroines in an H-scene.
        /// </summary>
        [HideFromIl2Cpp]
        protected internal abstract int MaxHeroineCount { get; }

        /// <summary>
        /// Set to true when the characters are REALLY going at it.
        /// </summary>
        [HideFromIl2Cpp]
        protected abstract bool IsHardSex { get; }

        /// <summary>
        /// Array of all penis base bones in the H-scene. <br/>
        /// The order of the bones need not be consistent.
        /// </summary>
        [HideFromIl2Cpp]
        protected internal virtual Transform[] PenisBases => new[] { PenisBase };

        /// <summary>
        /// Approximate length of a penis in Unity's length units.
        /// </summary>
        [HideFromIl2Cpp]
        protected internal virtual float PenisSize => 0f;

        /// <summary>
        /// Minimum length, relative to the full movement length, that counts as a stroke.
        /// </summary>
        protected internal virtual float MinStrokeLength => 0.5f;

        /// <summary>
        /// Override this to control the maximum vibration intensity. <br/>
        /// Value must be between 0 (=no vibration) and 1 (=full intensity).
        /// </summary>
        [HideFromIl2Cpp]
        protected internal virtual float VibrationIntensity => 1f;

        /// <summary>
        /// Override this to control the speed of down-strokes. <br/>
        /// Value must be between 0 (=normal speed) and 1 (=2x speed).
        /// </summary>
        [HideFromIl2Cpp]
        protected internal virtual float StrokingIntensity =>
            IsHardSex ? Mathf.InverseLerp(0f, 100f, StrokerConfig.HardSexIntensity.Value) : 0f;

        /// <summary>
        /// The shortest duration an orgasm should last. <br/>
        /// Override this if in-game orgasms don't have a detectable end,
        /// or if they're just too short.
        /// </summary>
        [HideFromIl2Cpp]
        protected internal virtual float MinOrgasmDurationSecs => 0f;

        /// <summary>
        /// Wrapper of Time.timeScale for testability.
        /// </summary>
        [HideFromIl2Cpp]
        protected internal virtual float TimeScale => Time.timeScale;

        internal bool IsHSceneRunning { get; private set; }

        /// <summary>
        /// The animator of the heroine at the given index. <br/>
        /// If the game doesn't use Animators, override GetAnimState instead. <br/>
        /// This will be called often, so keep it lightweight!
        /// </summary>
        [HideFromIl2Cpp]
        protected abstract Animator GetFemaleAnimator(int girlIndex);

        /// <summary>
        /// The root bone of this heroine. <br/>
        /// All bone names will be searched for inside this game object. <br/>
        /// If null, the search will be extended to the entire game.
        /// </summary>
        [HideFromIl2Cpp]
        protected abstract GameObject GetFemaleRoot(int girlIndex);

        /// <summary>
        /// A unique ID of the animation this girl is currently playing. <br/>
        /// This will be called often, so keep it lightweight!
        /// </summary>
        [HideFromIl2Cpp]
        protected internal abstract string GetPose(int girlIndex);

        /// <summary>
        /// True if this girl is currently NOT engaging in sex,
        /// false otherwise. <br/>
        /// This will be called often, so keep it lightweight!
        /// </summary>
        [HideFromIl2Cpp]
        protected internal abstract bool IsIdle(int girlIndex);

        /// <summary>
        /// A coroutine that waits until the H-scene is fully initialized. <br/>
        /// You might also want to set up your fields here, if you have any.
        /// </summary>
        [HideFromIl2Cpp]
        protected abstract IEnumerator UntilReady();

        /// <summary>
        /// The instance on which the method in StartHMethods was called
        /// will be passed to this method.
        /// </summary>
        [HideFromIl2Cpp]
        protected virtual void SetStartHInstance(object instance)
        { }

        /// <summary>
        /// Override this if the game has long cross-fade sections between
        /// animations, to wait until the cross-fade is over.
        /// </summary>
        [HideFromIl2Cpp]
        protected internal virtual IEnumerator WaitAfterPoseChange()
        {
            yield return new WaitForSeconds(0.1f);
        }

        /// <summary>
        /// Low-level replacement for Unity's AnimatorStateInfo. <br/>
        /// You might need this e.g. when animations are handled by Playables.
        /// </summary>
        /// <param name="girlIndex">
        /// the index of the heroine whose current animator state to get
        /// </param>
        /// <param name="normalizedTime">
        /// equivalent to AnimatorStateInfo.normalizedTime; make sure it always
        /// keeps increasing as long as the same animation is playing
        /// </param>
        /// <param name="length">equivalent to AnimatorStateInfo.length</param>
        /// <param name="speed">
        /// equivalent to AnimatorStateInfo.speed; must be relative to in-game
        /// time (i.e. ignoring timeScale) <br/>
        /// This will be called often, so keep it lightweight!
        /// </param>
        [HideFromIl2Cpp]
        protected internal virtual void GetAnimState(int girlIndex, out float normalizedTime,
            out float length, out float speed)
        {
            var info = GetAnimatorStateInfo(girlIndex);
            normalizedTime = info.normalizedTime;
            length = info.length;
            speed = info.speed;
        }

        /// <summary>
        /// True if this heroine (or the player) is currently orgasming;
        /// false otherwise. <br/>
        /// This will be called often, so keep it lightweight!
        /// </summary>
        [HideFromIl2Cpp]
        protected internal virtual bool IsOrgasming(int girlIndex) => false;

        internal void StartH(object instance)
        {
            EndH();
            IsHSceneRunning = true;
            SetStartHInstance(instance);
            HandleCoroutine(StartHWhenReady());
        }

        internal void EndH()
        {
            IsHSceneRunning = false;
            StopAllCoroutines();
            OnHEnded.Invoke(this, new HEventArgs());
            CoreConfig.Logger.LogInfo("H scene ended.");
        }

        private IEnumerator StartHWhenReady()
        {
            yield return HandleCoroutine(UntilReady());
            OnHStarted.Invoke(this, new HEventArgs());
            CoreConfig.Logger.LogInfo("New H scene started.");
        }

        protected AnimatorStateInfo GetAnimatorStateInfo(int girlIndex) =>
            GetFemaleAnimator(girlIndex).GetCurrentAnimatorStateInfo(AnimationLayer);

        internal float GetAnimationTimeSecs(int girlIndex)
        {
            GetAnimState(girlIndex, out _, out float length, out float speed);
            float animTimeSecs = length / speed / TimeScale;
            // prevent coroutines from hanging e.g. when the game is paused
            return animTimeSecs > 100f || animTimeSecs < 0.001f || float.IsNaN(animTimeSecs)
                ? .01f
                : animTimeSecs;
        }
        
        internal Dictionary<Bone, Transform> GetFemaleBones(int girlIndex) => FemaleBoneNames
            .ToDictionary(kvp => kvp.Key,
                kvp => FindBoneByPath(GetFemaleRoot(girlIndex), kvp.Value));

        protected static Transform FindBoneByPath(GameObject character, string path) =>
            // Search children
            character?.transform.Find(path)
                // If that fails, search recursively
                ?? FindDeepChildrenByName(character, path.Split('/').Last()).FirstOrDefault()
                    // If even that fails, search the entire game
                    ?? GameObject.Find(path.Split('/').Last()).transform;

        protected static Transform[] FindDeepChildrenByName(GameObject character, string name) =>
            character?
                .GetComponentsInChildren<Transform>()?
                .Where(child => child.name == name)
                .ToArray() ?? new Transform[] {};

        public class HEventArgs : EventArgs
        { }
    }
}