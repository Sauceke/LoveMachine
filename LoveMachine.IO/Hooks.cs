using System;
using BepInEx.Bootstrap;
using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.IO
{
    internal class Hooks
    {
        public static void InstallHooks()
        {
            var twosome = Type.GetType("FH_SetUp, Assembly-CSharp");
            var awake = new HarmonyMethod(AccessTools.Method(typeof(HSceneTriggers),
                nameof(HSceneTriggers.Awake)));
            var unload = new HarmonyMethod(AccessTools.Method(typeof(HSceneTriggers),
                nameof(HSceneTriggers.Unload)));
            var harmony = new Harmony(typeof(Hooks).FullName);
            harmony.Patch(AccessTools.Method(twosome, "Awake"), postfix: awake);
            harmony.Patch(AccessTools.Method(twosome, "Unload"), prefix: unload);
        }

        private static class HSceneTriggers
        {
            public static void Awake(MonoBehaviour __instance)
            {
                CoreConfig.Logger.LogDebug($"H Scene started: {__instance.name}.");
                // changing pose triggers this again, so stop monitoring first
                Array.ForEach(
                    Chainloader.ManagerObject.GetComponents<InsultOrderButtplugController>(),
                    ctrl => ctrl.OnEndH());
                Array.ForEach(
                    Chainloader.ManagerObject.GetComponents<InsultOrderButtplugController>(),
                    ctrl => ctrl.OnStartH());
            }

            public static void Unload(MonoBehaviour __instance)
            {
                CoreConfig.Logger.LogDebug($"H Scene ended: {__instance.name}.");
                Array.ForEach(
                    Chainloader.ManagerObject.GetComponents<InsultOrderButtplugController>(),
                    ctrl => ctrl.OnEndH());
            }
        }
    }
}
