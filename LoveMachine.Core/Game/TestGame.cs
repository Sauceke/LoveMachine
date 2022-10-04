using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoveMachine.Core
{
    internal sealed class TestGame : GameDescriptor
    {
        private float normalizedTime;
        private float speed;

        public TestGame(IntPtr handle) : base(handle) { }

        public override int AnimationLayer => throw new NotImplementedException();
        protected internal override Dictionary<Bone, string> FemaleBoneNames => throw new NotImplementedException();
        protected internal override int HeroineCount => throw new NotImplementedException();
        protected internal override int MaxHeroineCount => throw new NotImplementedException();
        protected internal override bool IsHardSex => false;
        protected internal override bool IsHSceneInterrupted => false;

        public override Animator GetFemaleAnimator(int girlIndex) => throw new NotImplementedException();
        protected internal override Transform GetDickBase() => throw new NotImplementedException();
        protected internal override GameObject GetFemaleRoot(int girlIndex) => throw new NotImplementedException();
        protected internal override string GetPose(int girlIndex) => throw new NotImplementedException();
        protected internal override bool IsIdle(int girlIndex) => false;
        protected internal override IEnumerator UntilReady() => throw new NotImplementedException();

        public IEnumerator RunTest(int strokes, float strokesPerSec)
        {
            speed = strokesPerSec;
            normalizedTime = 0f;
            // can't use Time.deltaTime here because some games
            // (KKS) set Time.timeScale to 0 while in the menu
            const float deltaTime = 1f / 30f;
            while (normalizedTime < strokes)
            {
                normalizedTime += deltaTime * speed;
                yield return new WaitForSecondsRealtime(deltaTime);
            }
        }

        protected internal override void GetAnimState(int girlIndex, out float normalizedTime,
            out float length, out float speed)
        {
            normalizedTime = this.normalizedTime;
            length = 1f;
            speed = this.speed;
        }
    }
}
