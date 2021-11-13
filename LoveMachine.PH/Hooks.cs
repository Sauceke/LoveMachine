using System;
using BepInEx.Bootstrap;
using HarmonyLib;
using LoveMachine.Core;

namespace LoveMachine.PH
{
    internal class Hooks
    {
        public static void InstallHooks()
        {
            Harmony.CreateAndPatchAll(typeof(HSceneTriggers));
        }

        private static class HSceneTriggers
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(H_Scene), nameof(H_Scene.Awake))]
            public static void Awake(H_Scene __instance)
            {
                CoreConfig.Logger.LogDebug("H Scene started.");
                Array.ForEach(
                    Chainloader.ManagerObject.GetComponents<PlayHomeButtplugController>(),
                    ctrl => ctrl.OnStartH(__instance));
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(H_Scene), nameof(H_Scene.Exit))]
            public static void Exit()
            {
                CoreConfig.Logger.LogDebug("H Scene ended.");
                Array.ForEach(
                    Chainloader.ManagerObject.GetComponents<PlayHomeButtplugController>(),
                    ctrl => ctrl.OnEndH());
            }
        }
    }
}
