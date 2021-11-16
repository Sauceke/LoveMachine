using System;
using BepInEx.Bootstrap;
using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.AGH
{
    internal class Hooks
    {
        public static void InstallHooks()
        {
            var fhSetUp = Type.GetType("FH_SetUp, Assembly-CSharp");
            var awake = new HarmonyMethod(AccessTools.Method(typeof(HSceneTriggers),
                nameof(HSceneTriggers.Awake)));
            var unload = new HarmonyMethod(AccessTools.Method(typeof(HSceneTriggers),
                nameof(HSceneTriggers.Awake)));
            var harmony = new Harmony(typeof(Hooks).FullName);
            harmony.Patch(AccessTools.Method(fhSetUp, "Awake"), postfix: awake);
            harmony.Patch(AccessTools.Method(fhSetUp, "Unload"), postfix: unload);
        }

        private static class HSceneTriggers
        {
            public static void Awake()
            {
                CoreConfig.Logger.LogDebug("H Scene started.");
                // changing pose triggers this again, so stop monitoring first
                Array.ForEach(
                    Chainloader.ManagerObject.GetComponents<HoukagoRinkanButtplugController>(),
                    ctrl => ctrl.OnEndH());
                Array.ForEach(
                    Chainloader.ManagerObject.GetComponents<HoukagoRinkanButtplugController>(),
                    ctrl => ctrl.OnStartH());
            }

            public static void Unload()
            {
                CoreConfig.Logger.LogDebug("H Scene ended.");
                Array.ForEach(
                    Chainloader.ManagerObject.GetComponents<HoukagoRinkanButtplugController>(),
                    ctrl => ctrl.OnEndH());
            }
        }
    }
}
