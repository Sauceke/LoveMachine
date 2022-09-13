using BepInEx.Bootstrap;
using HarmonyLib;
using LoveMachine.Core;

namespace LoveMachine.KK
{
    internal static class Hooks
    {
        public static void InstallHSceneHooks() =>
            Harmony.CreateAndPatchAll(typeof(HSceneTriggers));

        public static void InstallStudioHooks() =>
            Harmony.CreateAndPatchAll(typeof(StudioTriggers));

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

        private static class StudioTriggers
        {

            [HarmonyPostfix]
            [HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.InitScene))]
            public static void InitScene()
            {
                CoreConfig.Logger.LogDebug("Studio scene started.");
                Chainloader.ManagerObject.GetComponent<StudioGame>().EndH();
                Chainloader.ManagerObject.GetComponent<StudioGame>().StartH();
            }
        }
    }
}
