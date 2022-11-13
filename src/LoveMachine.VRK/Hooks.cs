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
            public static void StartH(VK_H_Houshi_Sonyu __instance) =>
                CoreConfig.ManagerObject.GetComponent<VRKanojoGame>().StartH(__instance);

            [HarmonyPrefix]
            [HarmonyPatch(typeof(VK_H_Houshi_Sonyu), nameof(VK_H_Houshi_Sonyu.OnDestroy))]
            public static void EndH() =>
                CoreConfig.ManagerObject.GetComponent<VRKanojoGame>().EndH();
        }
    }
}