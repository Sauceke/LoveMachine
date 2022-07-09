using System;
using BepInEx.Bootstrap;
using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.OA
{
    internal static class Hooks
    {
        public static void InstallHooks()
        {
            var sexManager = Type.GetType("SexSimManager, Assembly-CSharp");
            var start = new HarmonyMethod(AccessTools.Method(typeof(HSceneTriggers),
                nameof(HSceneTriggers.Start)));
            var harmony = new Harmony(typeof(Hooks).FullName);
            harmony.Patch(AccessTools.Method(sexManager, "Start"), postfix: start);
        }

        private static class HSceneTriggers
        {
            public static void Start(MonoBehaviour __instance)
            {
                CoreConfig.Logger.LogDebug("H Scene started.");
                Chainloader.ManagerObject.GetComponent<OurApartmentGame>().OnStartH(__instance);
            }
        }
    }
}
