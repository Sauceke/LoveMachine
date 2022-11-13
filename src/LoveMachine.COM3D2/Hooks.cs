using HarmonyLib;
using LoveMachine.Core;
using System;

namespace LoveMachine.COM3D2
{
    internal static class Hooks
    {
        public static void InstallHooks()
        {
            var yotogi = Type.GetType("YotogiPlayManager, Assembly-CSharp");
            var startH = new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(StartH)));
            var endH = new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(EndH)));
            var harmony = new Harmony(typeof(Hooks).FullName);
            harmony.Patch(AccessTools.Method(yotogi, "UIStartup"), postfix: startH);
            harmony.Patch(AccessTools.Method(yotogi, "OnClickNext"), prefix: endH);
        }

        public static void StartH() =>
            CoreConfig.ManagerObject.GetComponent<Com3d2Game>().StartH();

        public static void EndH() =>
            CoreConfig.ManagerObject.GetComponent<Com3d2Game>().EndH();
    }
}