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

        }
    }
}
