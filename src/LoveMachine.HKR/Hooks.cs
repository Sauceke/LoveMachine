using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.HKR
{
    internal static class Hooks
    {
        public static void InstallHooks()
        {
            var uiController = Type.GetType("ATD.UIController, ATDAssemblyDifinition");
            var startH = new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(StartH)));
            var endH = new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(EndH)));
            var harmony = new Harmony(typeof(Hooks).FullName);
            harmony.Patch(AccessTools.Method(uiController, "FinishADV"), postfix: startH);
            harmony.Patch(AccessTools.Method(uiController, "OnDestroy"), prefix: endH);
        }

        public static void StartH(MonoBehaviour __instance) =>
            CoreConfig.ManagerObject.GetComponent<HolyKnightRiccaGame>().StartH(__instance);

        public static void EndH() =>
            CoreConfig.ManagerObject.GetComponent<HolyKnightRiccaGame>().EndH();
    }
}