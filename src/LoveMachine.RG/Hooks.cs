using HarmonyLib;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.RG
{
    internal class Hooks
    {
        public static void InstallHooks()
        {
            var hScene = Type.GetType("HScene, Assembly-CSharp");
            var startH = new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(StartH)));
            var endH = new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(EndH)));
            var harmony = new Harmony(typeof(Hooks).FullName);
            harmony.Patch(AccessTools.Method(hScene, "Start"), postfix: startH);
            harmony.Patch(AccessTools.Method(hScene, "OnDestroy"), prefix: endH);
        }

        public static void StartH(MonoBehaviour __instance) =>
            CoreConfig.ManagerObject.GetComponent<RoomGirlGame>().StartH(__instance);

        public static void EndH() =>
            CoreConfig.ManagerObject.GetComponent<RoomGirlGame>().EndH();
    }
}