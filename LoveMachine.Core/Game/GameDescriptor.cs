using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoveMachine.Core
{
    public abstract class GameDescriptor : MonoBehaviour
    {
        protected internal event EventHandler<HEventArgs> OnHStarted;
        protected internal event EventHandler<HEventArgs> OnHEnded;

        private bool hRunning = false;

        protected internal abstract Dictionary<Bone, string> FemaleBoneNames { get; }
        public abstract int AnimationLayer { get; }
        protected internal abstract int HeroineCount { get; }
        protected internal abstract int MaxHeroineCount { get; }
        protected internal abstract bool IsHardSex { get; }
        protected internal abstract bool IsHSceneInterrupted { get; }

        protected internal virtual float PenisSize => 0f;

        protected internal virtual float VibrationIntensity => 1f;

        protected internal virtual float StrokingIntensity =>
            IsHardSex ? Mathf.InverseLerp(0f, 100f, StrokerConfig.HardSexIntensity.Value) : 0f;

        internal bool IsHSceneRunning => hRunning && !IsHSceneInterrupted;

        public abstract Animator GetFemaleAnimator(int girlIndex);
        protected internal abstract GameObject GetFemaleRoot(int girlIndex);
        protected internal abstract Transform GetDickBase();
        protected internal abstract string GetPose(int girlIndex);
        protected internal abstract bool IsIdle(int girlIndex);
        protected internal abstract IEnumerator UntilReady();

        protected internal virtual IEnumerator WaitAfterPoseChange()
        {
            yield return new WaitForSeconds(0.1f);
        }

        protected internal virtual void GetAnimState(int girlIndex, out float normalizedTime,
            out float length, out float speed)
        {
            var info = GetAnimatorStateInfo(girlIndex);
            normalizedTime = info.normalizedTime;
            length = info.length;
            speed = info.speed;
        }

        protected internal virtual bool IsOrgasming(int girlIndex) => false;

        public void StartH()
        {
            hRunning = true;
            OnHStarted.Invoke(this, new HEventArgs());
        }

        public void EndH()
        {
            hRunning = false;
            OnHEnded.Invoke(this, new HEventArgs());
        }

        public AnimatorStateInfo GetAnimatorStateInfo(int girlIndex) =>
            GetFemaleAnimator(girlIndex).GetCurrentAnimatorStateInfo(AnimationLayer);

        internal Dictionary<Bone, Transform> GetFemaleBones(int girlIndex) => FemaleBoneNames
            .ToDictionary(kvp => kvp.Key,
                kvp => FindBoneByPath(GetFemaleRoot(girlIndex), kvp.Value));

        protected static Transform FindBoneByPath(GameObject character, string path) =>
            // Find the root character object
            character?.transform?.Find(path)
                // If the program can not find the component, it will try to use the name of the
                // component to match every child of the root chara by recursion
                ?? FindDeepChildByName(character, path.Split('/').Last())
                    // If even that fails, search the entire game
                    ?? GameObject.Find(path.Split('/').Last()).transform;

        private static Transform FindDeepChildByName(GameObject character, string name) =>
            character?.GetComponentsInChildren<Transform>()?
                .FirstOrDefault(child => child.name == name);

        public class HEventArgs : EventArgs { }
    }
}
