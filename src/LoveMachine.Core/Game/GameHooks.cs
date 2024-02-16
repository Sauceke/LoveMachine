using System;
using HarmonyLib;
using LoveMachine.Core.NonPortable;

namespace LoveMachine.Core.Game
{
    internal static class GameHooks
    {
        private static GameAdapter Game => Globals.ManagerObject.GetComponent<GameAdapter>();

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