using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.OT
{
    internal class OedoTriggerGame : TimelineGameDescriptor
    {
        private GameObject[] femaleRoots;
        
        protected override MethodInfo[] StartHMethods =>
            new[] { AccessTools.Method("CQCDirection, NKAssets:PlayCQC") };

        protected override MethodInfo[] EndHMethods =>
            new[] { AccessTools.Method("GalleryController, NKAssets:EndGallery") };

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "PssyPos" },
            { Bone.Mouth, "MouthPos" },
            { Bone.LeftHand, "DEF-f_index.03.L" },
            { Bone.RightHand, "DEF-f_index.03.R" },
            { Bone.LeftFoot, "DEF-toe.L" },
            { Bone.RightFoot, "DEF-toe.R" },
            { Bone.LeftBreast, "DEF-Breast.L" },
            { Bone.RightBreast, "DEF-Breast.R" }
        };

        protected override Transform PenisBase => GameObject.Find(
            "SubSystem/NKDirection/PlayableDirector/BoyOriginal/rig/root/DEF-spine/Penis0")
            .transform;

        protected override int HeroineCount => femaleRoots.Length;

        protected override int MaxHeroineCount => 3;

        protected override bool IsHardSex => false;

        protected override GameObject GetFemaleRoot(int girlIndex) => femaleRoots[girlIndex];

        protected override bool IsIdle(int girlIndex) => false;

        protected override Traverse Director => Traverse.Create(FindObjectOfType(
            Type.GetType("UnityEngine.Playables.PlayableDirector, UnityEngine.DirectorModule")));

        protected override Traverse Timeline => Director.Property("playableAsset");

        protected override string TrackName => "Loop Track";

        protected override IEnumerator UntilReady()
        {
            yield return base.UntilReady();
            femaleRoots = Enumerable.Range(1, 3)
                .Select(i => GameObject.Find(
                    $"SubSystem/NKDirection/PlayableDirector/CQC_GirlOriginal_G{i}/rig/root"))
                .Where(root => root != null && root.activeInHierarchy)
                .ToArray();
        }
    }
}