using Il2CppInterop.Runtime.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private bool hRunning = false;

        /// <summary>
        /// The name/path of each bone's GameObject in female characters. <br/>
        /// Not all bones are required to have an entry. <br/>
        /// Do NOT add an entry for Bone.Auto!
        /// </summary>
        [HideFromIl2Cpp]
        protected internal abstract Dictionary<Bone, string> FemaleBoneNames { get; }

        /// <summary>
        /// The layer index of the sex animation in female animators
        /// (if the game uses animators).
        /// </summary>
        [HideFromIl2Cpp]
        public abstract int AnimationLayer { get; }

        /// <summary>
        /// Number of heroines in the current H-scene.
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
        protected internal abstract bool IsHardSex { get; }

        /// <summary>
        /// Indicates that the H-scene has ended (or you can call EndH instead).
        /// </summary>
        [HideFromIl2Cpp]
        protected internal abstract bool IsHSceneInterrupted { get; }

        /// <summary>
        /// Approximate length of a penis in Unity's length units.
        /// </summary>
        [HideFromIl2Cpp]
        protected internal virtual float PenisSize => 0f;

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

        internal bool IsHSceneRunning => hRunning && !IsHSceneInterrupted;

        /// <summary>
        /// The animator of the heroine at the given index. <br/>
        /// If the game doesn't use Animators, override GetAnimState instead.
        /// </summary>
        [HideFromIl2Cpp]
        public abstract Animator GetFemaleAnimator(int girlIndex);

        /// <summary>
        /// The root bone of this heroine. <br/>
        /// All bone names will be searched for inside this game object. <br/>
        /// If null, the search will be extended to the entire game.
        /// </summary>
        [HideFromIl2Cpp]
        protected internal abstract GameObject GetFemaleRoot(int girlIndex);

        /// <summary>
        /// The base of the penis or something close to that (e.g. balls).
        /// </summary>
        [HideFromIl2Cpp]
        protected internal abstract Transform GetDickBase();

        /// <summary>
        /// A unique ID of the animation this girl is currently playing. <br/>
        /// This will be called often, so keep it lightweight!
        /// </summary>
        [HideFromIl2Cpp]
        protected internal abstract string GetPose(int girlIndex);

        /// <summary>
        /// True if this girl is currently NOT engaging in sex,
        /// false otherwise.
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
        /// Override this if the game has long crossfade sections between
        /// animations, to wait until the crossfade is over.
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
        /// time (i.e. ignoring timeScale)
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
        /// false otherwise.
        /// </summary>
        [HideFromIl2Cpp]
        protected internal virtual bool IsOrgasming(int girlIndex) => false;

        /// <summary>
        /// Call this when the H-scene starts.
        /// </summary>
        public void StartH()
        {
            EndH();
            hRunning = true;
            HandleCoroutine(StartHWhenReady());
        }

        /// <summary>
        /// Call this or set IsHSceneInterrupted to true when the H-scene ends.
        /// </summary>
        public void EndH()
        {
            hRunning = false;
            OnHEnded.Invoke(this, new HEventArgs());
            CoreConfig.Logger.LogInfo("H scene ended.");
        }

        private IEnumerator StartHWhenReady()
        {
            yield return HandleCoroutine(UntilReady());
            OnHStarted.Invoke(this, new HEventArgs());
            CoreConfig.Logger.LogInfo("New H scene started.");
        }

        public AnimatorStateInfo GetAnimatorStateInfo(int girlIndex) =>
            GetFemaleAnimator(girlIndex).GetCurrentAnimatorStateInfo(AnimationLayer);

        internal Dictionary<Bone, Transform> GetFemaleBones(int girlIndex) => FemaleBoneNames
            .ToDictionary(kvp => kvp.Key,
                kvp => FindBoneByPath(GetFemaleRoot(girlIndex), kvp.Value));

        protected static Transform FindBoneByPath(GameObject character, string path) =>
            // Search children
            character?.transform?.Find(path)
                // If that fails, search recursively
                ?? FindDeepChildByName(character, path.Split('/').Last())
                    // If even that fails, search the entire game
                    ?? GameObject.Find(path.Split('/').Last()).transform;

        private static Transform FindDeepChildByName(GameObject character, string name) =>
            character?.GetComponentsInChildren<Transform>()?
                .FirstOrDefault(child => child.name == name);

        public class HEventArgs : EventArgs
        { }
    }
}