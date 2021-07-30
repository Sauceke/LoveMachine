using BepInEx.Bootstrap;
using HarmonyLib;

namespace ButtPlugin.HS2
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
            [HarmonyPatch(typeof(HScene), nameof(HScene.Start))]
            public static void Start(HScene __instance)
            {
                Chainloader.ManagerObject.GetComponent<HoneySelect2ButtplugStrokerController>()
                    .OnStartH(__instance);
                Chainloader.ManagerObject.GetComponent<HoneySelect2ButtplugVibrationController>()
                    .OnStartH(__instance);
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(HScene), nameof(HScene.EndProc))]
            public static void End()
            {
                Chainloader.ManagerObject.GetComponent<HoneySelect2ButtplugStrokerController>()
                    .OnEndH();
                Chainloader.ManagerObject.GetComponent<HoneySelect2ButtplugVibrationController>()
                    .OnEndH();
            }
        }
    }
}
