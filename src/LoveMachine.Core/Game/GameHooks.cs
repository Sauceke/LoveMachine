using HarmonyLib;
using System;

namespace LoveMachine.Core
{
    internal static class GameHooks
    {
        private static GameDescriptor Game =>
            CoreConfig.ManagerObject.GetComponent<GameDescriptor>();

        public static void InstallHooks()
        {
            var startH = new HarmonyMethod(AccessTools.Method(typeof(GameHooks), nameof(StartH)));
            var endH = new HarmonyMethod(AccessTools.Method(typeof(GameHooks), nameof(EndH)));
            var harmony = new Harmony(typeof(GameHooks).FullName);
            Array.ForEach(Game.StartHMethods, method => harmony.Patch(method, postfix: startH));
            Array.ForEach(Game.EndHMethods, method => harmony.Patch(method, prefix: endH));
        }

        public static void StartH(object __instance) => Game.StartH(__instance);

        public static void EndH() => Game.EndH();
    }
}