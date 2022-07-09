using System;
using BepInEx.Bootstrap;
using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.COM3D2
{
    internal class Hooks
    {
        public static void InstallHooks()
        {
            var yotogi = Type.GetType("YotogiPlayManager, Assembly-CSharp");
            var start = new HarmonyMethod(AccessTools.Method(typeof(HSceneTriggers),
                nameof(HSceneTriggers.Start)));
            var end = new HarmonyMethod(AccessTools.Method(typeof(HSceneTriggers),
                nameof(HSceneTriggers.End)));
            var harmony = new Harmony(typeof(Hooks).FullName);
            harmony.Patch(AccessTools.Method(yotogi, "UIStartup"), postfix: start);
            harmony.Patch(AccessTools.Method(yotogi, "OnClickNext"), prefix: end);
        }

        private static class HSceneTriggers
        {
            public static void Start(MonoBehaviour __instance)
            {
                CoreConfig.Logger.LogDebug($"H Scene started: {__instance.name}.");
                Chainloader.ManagerObject.GetComponent<Com3d2Game>().StartH();
            }

            public static void End(MonoBehaviour __instance)
            {
                CoreConfig.Logger.LogDebug($"H Scene ended: {__instance.name}.");
                Chainloader.ManagerObject.GetComponent<Com3d2Game>().EndH();
            }
        }
    }
}
