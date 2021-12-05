using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.OA
{
    public abstract class OurApartmentButtplugController : ButtplugController
    {
        internal static readonly Dictionary<string, string> femaleBones
            = new Dictionary<string, string>
            {
                { "cc_pussy_clit", "Pussy" },
                { "cc_boob.l", "Left Breast" },
                { "cc_boob.r", "Right Breast" },
                { "c_teeth_top.x", "Mouth" },
                { "c_toes_thumb1.l", "Left Foot" },
                { "c_toes_thumb1.r", "Right Foot" },
                { "index1.l", "Left Hand" },
                { "index1.r", "Right Hand" }
            };
        private const string MaleBoneName = "cc_balls1.l"; 

        private Traverse<Animator> npcAnimator;
        private Traverse<bool> isSex;
        private Traverse<bool> isGrind;
        private Traverse<float> speed;
        private Traverse<int> maxSpeed;

        public void OnStartH(MonoBehaviour manager)
        {
            var managerTraverse = Traverse.Create(manager);
            npcAnimator = managerTraverse.Field<Animator>("npcAnimator");
            isSex = managerTraverse.Field<bool>("isSex");
            isGrind = managerTraverse.Field<bool>("isGrind");
            speed = managerTraverse.Field("mainPlayer").Field<float>("currentSpeed");
            maxSpeed = managerTraverse.Field("mainPlayer").Field<int>("maxSpeed");
            OnStartH();
        }

        protected override int HeroineCount => 1; // Just Naomi

        protected override bool IsHardSex =>
            speed.Value > maxSpeed.Value / 2f && !isGrind.Value;

        protected override int AnimationLayer
        {
            get
            {
                var animator = npcAnimator.Value;
                for (int i=0; i<animator.layerCount; i++)
                {
                    if (animator.GetLayerWeight(i) == 1f)
                    {
                        return i;
                    }
                }
                throw new Exception("Active animation layer not found");
            }
        }

        protected override int CurrentAnimationStateHash => throw new NotImplementedException();

        protected override bool IsHSceneInterrupted => false;

        protected override Animator GetFemaleAnimator(int girlIndex) => npcAnimator.Value;

        protected override List<Transform> GetFemaleBones(int girlIndex)
            => femaleBones.Keys
                .Select(name => GameObject.Find(name).transform)
                .ToList();

        protected override Transform GetMaleBone() => GameObject.Find(MaleBoneName).transform;

        protected override string GetPose(int girlIndex)
            => npcAnimator.Value.GetCurrentAnimatorClipInfo(AnimationLayer)[0].clip.name;

        protected override int GetStrokesPerAnimationCycle(int girlIndex) => 1;

        protected override bool IsIdle(int girlIndex) => !isSex.Value;

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSecondsRealtime(5f);
        }
    }

    public class OurApartmentButtplugVibrationController : OurApartmentButtplugController
    {
        protected override IEnumerator Run(int girlIndex, int boneIndex)
            => RunVibratorLoop(girlIndex, boneIndex);
    }

    public class OurApartmentButtplugStrokerController : OurApartmentButtplugController
    {
        protected override IEnumerator Run(int girlIndex, int boneIndex)
            => RunStrokerLoop(girlIndex, boneIndex);
    }
}
