using System;
using BepInEx.Bootstrap;
using HarmonyLib;
using LoveMachine.Core;

namespace LoveMachine.HS2
{
    internal static class Hooks
    {
        public static void InstallHooks() => Harmony.CreateAndPatchAll(typeof(HSceneTriggers));

        private static class HSceneTriggers
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(HScene), nameof(HScene.Start))]
            public static void Start(HScene __instance)
            {
                CoreConfig.Logger.LogDebug("H Scene started.");
                Array.ForEach(
                    Chainloader.ManagerObject.GetComponents<HoneySelect2Game>(),
                    game => game.OnStartH(__instance));
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(HScene), nameof(HScene.EndProc))]
            public static void End()
            {
                CoreConfig.Logger.LogDebug("H Scene ended.");
                Array.ForEach(
                    Chainloader.ManagerObject.GetComponents<HoneySelect2Game>(),
                    game => game.EndH());
            }
        }
    }
}
