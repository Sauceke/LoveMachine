using System;
using BepInEx.Bootstrap;
using HarmonyLib;
using LoveMachine.Core;

namespace LoveMachine.KK
{
    internal static class Hooks
    {
        public static void InstallHooks()
        {
            Harmony.CreateAndPatchAll(typeof(HSceneTriggers));
        }

        private static class HSceneTriggers
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(HFlag), nameof(HFlag.Start))]
            public static void Start(HFlag __instance)
            {
                CoreConfig.Logger.LogDebug("H Scene started.");
                Array.ForEach(
                    Chainloader.ManagerObject.GetComponents<KoikatsuButtplugController>(),
                    ctrl => ctrl.OnStartH(__instance));
            }
        }
    }
}
