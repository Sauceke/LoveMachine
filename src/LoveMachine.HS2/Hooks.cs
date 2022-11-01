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
                Chainloader.ManagerObject.GetComponent<HoneySelect2Game>().OnStartH(__instance);
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(HScene), nameof(HScene.EndProc))]
            public static void End()
            {
                CoreConfig.Logger.LogDebug("H Scene ended.");
                Chainloader.ManagerObject.GetComponent<HoneySelect2Game>().EndH();
            }
        }
    }
}
