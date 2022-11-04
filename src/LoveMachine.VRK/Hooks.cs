using HarmonyLib;
using LoveMachine.Core;

namespace LoveMachine.VRK
{
    internal static class Hooks
    {
        public static void InstallHooks() => Harmony.CreateAndPatchAll(typeof(HSceneTriggers));

        private static class HSceneTriggers
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(VK_H_Houshi_Sonyu), nameof(VK_H_Houshi_Sonyu.Start))]
            public static void StartProc(VK_H_Houshi_Sonyu __instance)
            {
                CoreConfig.Logger.LogDebug("H Scene started.");
                CoreConfig.ManagerObject.GetComponent<VRKanojoGame>().OnStartH(__instance);
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(VK_H_Houshi_Sonyu), nameof(VK_H_Houshi_Sonyu.OnDestroy))]
            public static void End()
            {
                CoreConfig.Logger.LogDebug("H Scene ended.");
                CoreConfig.ManagerObject.GetComponent<VRKanojoGame>().EndH();
            }
        }
    }
}