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
        private static readonly Dictionary<Bone, string> femaleBones
            = new Dictionary<Bone, string>
            {
                { Bone.Vagina, "cc_pussy_clit" },
                { Bone.LeftBreast, "cc_boob.l" },
                { Bone.RightBreast, "cc_boob.r" },
                { Bone.Mouth, "c_teeth_top.x" },
                { Bone.LeftFoot, "c_toes_thumb1.l" },
                { Bone.RightFoot, "c_toes_thumb1.r" },
                { Bone.LeftHand, "index1.l" },
                { Bone.RightHand, "index1.r" }
            };
        private const string MaleBoneName = "cc_balls1.l";
        private static readonly string[] layerNames =
        {
            "Base SexSim", "From Behind SexSim", "Couch Missionary SexSim"
        };

        private Traverse<Animator> npcAnimator;
        private Traverse<bool> isSex;

        public void OnStartH(MonoBehaviour manager)
        {
            var managerTraverse = Traverse.Create(manager);
            npcAnimator = managerTraverse.Field<Animator>("npcAnimator");
            isSex = managerTraverse.Field<bool>("isSex");
            OnStartH();
        }

        protected override int HeroineCount => 1; // Just Naomi

        protected override bool IsHardSex => GetPose(0).Contains("Pump2");
        protected override int AnimationLayer
        {
            get
            {
                var animator = npcAnimator.Value;
                for (int i = 0; i < animator.layerCount; i++)
                {
                    if (layerNames.Contains(animator.GetLayerName(i))
                        && animator.GetLayerWeight(i) == 1f)
                    {
                        return i;
                    }
                }
                CoreConfig.Logger.LogWarning("Active animation layer not found");
                return 0;
            }
        }

        protected override bool IsHSceneInterrupted => false;

        protected override Animator GetFemaleAnimator(int girlIndex) => npcAnimator.Value;

        protected override Dictionary<Bone, Transform> GetFemaleBones(int girlIndex)
            => femaleBones.ToDictionary(kvp => kvp.Key,
                kvp => GameObject.Find(kvp.Value).transform);

        protected override Transform GetMaleBone() => GameObject.Find(MaleBoneName).transform;

        protected override string GetPose(int girlIndex)
            => npcAnimator.Value.GetCurrentAnimatorClipInfo(AnimationLayer)[0].clip.name;

        protected override bool IsIdle(int girlIndex) => !isSex.Value;

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSecondsRealtime(5f);
        }
    }

    public class OurApartmentButtplugVibrationController : OurApartmentButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone)
            => RunVibratorLoop(girlIndex, bone);
    }

    public class OurApartmentButtplugStrokerController : OurApartmentButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone)
            => RunStrokerLoop(girlIndex, bone);
    }
}
