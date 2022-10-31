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
            var start = new HarmonyMethod(AccessTools.Method(typeof(HSceneTriggers),
                nameof(HSceneTriggers.Start)));
            var finish = new HarmonyMethod(AccessTools.Method(typeof(HSceneTriggers),
                nameof(HSceneTriggers.Finish)));
            var harmony = new Harmony(typeof(Hooks).FullName);
            harmony.Patch(AccessTools.Method(uiController, "FinishADV"), postfix: start);
            harmony.Patch(AccessTools.Method(uiController, "OnDestroy"), postfix: finish);
        }

        private static class HSceneTriggers
        {
            public static void Start(MonoBehaviour __instance)
            {
                CoreConfig.Logger.LogDebug("H Scene started.");
                CoreConfig.ManagerObject.GetComponent<HolyKnightRiccaGame>().StartH(__instance);
            }

            public static void Finish()
            {
                CoreConfig.Logger.LogDebug("H Scene ended.");
                CoreConfig.ManagerObject.GetComponent<HolyKnightRiccaGame>().EndH();
            }
        }
    }
}
