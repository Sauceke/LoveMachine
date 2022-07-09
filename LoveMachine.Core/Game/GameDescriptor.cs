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

        public abstract int AnimationLayer { get; }
        protected internal abstract int HeroineCount { get; }
        protected internal abstract bool IsHardSex { get; }
        protected internal abstract bool IsHSceneInterrupted { get; }

        protected internal virtual float PenisSize => 0f;

        protected internal virtual float VibrationIntensity => 1f;

        public abstract Animator GetFemaleAnimator(int girlIndex);
        protected internal abstract Dictionary<Bone, Transform> GetFemaleBones(int girlIndex);
        protected internal abstract Transform GetMaleBone();
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

        public void StartH() => OnHStarted.Invoke(this, new HEventArgs());

        public void EndH() => OnHEnded.Invoke(this, new HEventArgs());

        protected internal IEnumerable<Bone> GetSupportedBones(int girlIndex) =>
            Enumerable.Concat(new[] { Bone.Auto }, GetFemaleBones(girlIndex).Keys);

        public AnimatorStateInfo GetAnimatorStateInfo(int girlIndex) =>
            GetFemaleAnimator(girlIndex).GetCurrentAnimatorStateInfo(AnimationLayer);

        public class HEventArgs : EventArgs { }
    }
}
