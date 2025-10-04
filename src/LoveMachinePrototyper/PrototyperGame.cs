using HarmonyLib;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LoveMachinePrototyper
{
    internal class PrototyperGame : GameAdapter
    {
        private Animator animator;
        private GameObject[] femaleRoots;

        protected override MethodInfo[] StartHMethods => new[]
        {
            AccessTools.Method(typeof(HListener), nameof(HListener.StartH))
        };

        protected override MethodInfo[] EndHMethods => new[]
        {
            AccessTools.Method(typeof(HListener), nameof(HListener.EndH))
        };

        protected override Dictionary<Bone, string> FemaleBoneNames =>
            PrototyperConfig.FemaleBoneNames
                .Where(entry => !string.IsNullOrEmpty(entry.Value.Value))
                .ToDictionary(entry => entry.Key, entry => entry.Value.Value);

        protected override Transform PenisBase => throw new System.NotImplementedException();

        protected override Transform[] PenisBases =>
            FindUtil.FindAll(PrototyperConfig.PenisBaseName.Value);

        protected override float MinStrokeLength => PrototyperConfig.StrokeSensitivity.Value;

        protected override int AnimationLayer => PrototyperConfig.AnimationLayer.Value;

        protected override int HeroineCount => Mathf.Max(femaleRoots.Length, 1);

        protected override int MaxHeroineCount => PrototyperConfig.MaxFemaleCount.Value;

        protected override bool IsHardSex => false;

        protected override Animator GetFemaleAnimator(int girlIndex) => animator;

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            girlIndex < femaleRoots.Length ? femaleRoots[girlIndex] : null;

        protected override string GetPose(int girlIndex) =>
            GetAnimatorStateInfo(girlIndex).fullPathHash.ToString();

        protected override bool IsIdle(int girlIndex) => false;

        protected override IEnumerator UntilReady(object instance)
        {
            yield return new WaitForSeconds(5f);
            animator = FindUtil.FindFirst(PrototyperConfig.AnimatorName.Value)
                .GetComponent<Animator>();
            femaleRoots = FindUtil.FindAll(PrototyperConfig.FemaleRootName.Value)
                .Select(tf => tf.gameObject).ToArray();
        }
    }
}
