using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.OA
{
    internal sealed class OurApartmentGame : GameDescriptor
    {
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
            StartH();
        }

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
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

        protected override int HeroineCount => 1; // Just Naomi

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => GetPose(0).Contains("Pump2");

        public override int AnimationLayer
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
                return -1;
            }
        }

        protected override bool IsHSceneInterrupted => false;

        protected override float PenisSize => 0.2f;

        public override Animator GetFemaleAnimator(int girlIndex) => npcAnimator.Value;

        protected override GameObject GetFemaleRoot(int girlIndex) => null;

        protected override Transform GetDickBase() =>
            GameObject.Find("cc_balls1.l")?.transform ?? transform;

        protected override string GetPose(int girlIndex) =>
            AnimationLayer < 0
                ? "unknown_pose"
                : npcAnimator.Value.GetCurrentAnimatorClipInfo(AnimationLayer)[0].clip.name;

        protected override bool IsIdle(int girlIndex) => !isSex.Value;

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSecondsRealtime(5f);
        }
    }
}
