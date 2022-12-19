using BepInEx.Bootstrap;
using HarmonyLib;

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
            public static void StartH(HFlag __instance) =>
                Chainloader.ManagerObject.GetComponent<KoikatsuGame>().StartH(__instance);

            [HarmonyPrefix]
            [HarmonyPatch(typeof(HFlag), nameof(HFlag.Destroy))]
            public static void EndH() =>
                Chainloader.ManagerObject.GetComponent<KoikatsuGame>().EndH();
        }

        private static class StudioTriggers
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.InitScene))]
            public static void StartH() =>
                Chainloader.ManagerObject.GetComponent<StudioGame>().StartH();
        }
    }
}