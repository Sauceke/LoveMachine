using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.OT
{
    internal class OedoTriggerGame : TimelineGameDescriptor
    {
        protected override MethodInfo[] StartHMethods =>
            new[] { AccessTools.Method("GalleryController, NKAssets:StartMotionPlay") };

        protected override MethodInfo[] EndHMethods =>
            new[] { AccessTools.Method("GalleryController, NKAssets:EndGallery") };

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "PssyPos" }
        };

        protected override Transform PenisBase => GameObject.Find(
            "SubSystem/NKDirection/PlayableDirector/BoyOriginal/rig/root/DEF-spine/Penis0")
            .transform;

        protected override int HeroineCount => 1;

        protected override int MaxHeroineCount => 3;

        protected override bool IsHardSex => false;

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            GameObject.Find("SubSystem/NKDirection/PlayableDirector/CQC_GirlOriginal_G1/rig/root");

        protected override bool IsIdle(int girlIndex) => false;

        protected override Traverse Director => Traverse.Create(FindObjectOfType(
            Type.GetType("UnityEngine.Playables.PlayableDirector, UnityEngine.DirectorModule")));

        protected override Traverse Timeline => Director.Property("playableAsset");

        protected override string TrackName => "Loop Track";
    }
}