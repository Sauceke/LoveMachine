using BepInEx.Bootstrap;
using HarmonyLib;
using LoveMachine.Core;

namespace LoveMachine.KK
{
    internal static class Hooks
    {
        public static void InstallHooks() => Harmony.CreateAndPatchAll(typeof(HSceneTriggers));

        private static class HSceneTriggers
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(HFlag), nameof(HFlag.Start))]
            public static void Start(HFlag __instance)
            {
                CoreConfig.Logger.LogDebug("H Scene started.");
                Chainloader.ManagerObject.GetComponent<KoikatsuGame>().OnStartH(__instance);
            }
        }
    }
}
