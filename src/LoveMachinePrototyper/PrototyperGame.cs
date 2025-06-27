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
    public class PrototyperGame : GameAdapter
    {
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

        protected override Transform PenisBase =>
            GameObject.Find(PrototyperConfig.PenisBaseName.Value).transform;

        protected override int AnimationLayer => PrototyperConfig.AnimationLayer.Value;

        protected override int HeroineCount => 1;

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => false;

        protected override Animator GetFemaleAnimator(int girlIndex) =>
            GameObject.Find(PrototyperConfig.AnimatorName.Value).GetComponent<Animator>();

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            GameObject.Find(PrototyperConfig.FemaleRootName.Value);

        protected override string GetPose(int girlIndex) =>
            GetAnimatorStateInfo(girlIndex).fullPathHash.ToString();

        protected override bool IsIdle(int girlIndex) => false;

        protected override IEnumerator UntilReady(object instance)
        {
            yield return new WaitForSeconds(5f);
        }
    }
}
