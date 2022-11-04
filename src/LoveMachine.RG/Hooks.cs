using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.RG
{
    internal class Hooks
    {
        public static void InstallHooks()
        {
            var sexManager = Type.GetType("HScene, Assembly-CSharp");
            var start = new HarmonyMethod(AccessTools.Method(typeof(HSceneTriggers),
                nameof(HSceneTriggers.Start)));
            var onDestroy = new HarmonyMethod(AccessTools.Method(typeof(HSceneTriggers),
                nameof(HSceneTriggers.OnDestroy)));
            var harmony = new Harmony(typeof(Hooks).FullName);
            harmony.Patch(AccessTools.Method(sexManager, "Start"), postfix: start);
            harmony.Patch(AccessTools.Method(sexManager, "OnDestroy"), postfix: onDestroy);
        }

        private static class HSceneTriggers
        {
            public static void Start(MonoBehaviour __instance)
            {
                CoreConfig.Logger.LogDebug("H Scene started.");
                CoreConfig.ManagerObject.GetComponent<RoomGirlGame>().StartH(__instance);
            }

            public static void OnDestroy()
            {
                CoreConfig.Logger.LogDebug("H Scene ended.");
                CoreConfig.ManagerObject.GetComponent<RoomGirlGame>().EndH();
            }
        }
    }
}