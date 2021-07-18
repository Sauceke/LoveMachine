using BepInEx.Bootstrap;
using HarmonyLib;

namespace KK_ButtPlugin
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
                Chainloader.ManagerObject.GetComponent<ButtplugVibrationController>()
                    .OnStartH(__instance);
                Chainloader.ManagerObject.GetComponent<ButtplugStrokerController>()
                    .OnStartH(__instance);
            }
        }
    }
}
