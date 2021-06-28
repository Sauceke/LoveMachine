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
            [HarmonyPrefix]
            [HarmonyPatch(typeof(HFlag), nameof(HFlag.SetInsertKokan))]
            [HarmonyPatch(typeof(HFlag), nameof(HFlag.SetInsertAnal))]
            [HarmonyPatch(typeof(HFlag), nameof(HFlag.SetHoushiPlay))]
            public static void Insert(HFlag __instance)
            {
                ButtPlugin.Controller.OnStart(__instance);
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(HFlag), nameof(HFlag.AddSonyuOrg))]
            [HarmonyPatch(typeof(HFlag), nameof(HFlag.AddSonyuAnalOrg))]
            [HarmonyPatch(typeof(HFlag), nameof(HFlag.AddHoushiInside))]
            [HarmonyPatch(typeof(HFlag), nameof(HFlag.AddHoushiOutside))]
            public static void Finish()
            {
                ButtPlugin.Controller.OnFinish();
            }
        }
    }
}
