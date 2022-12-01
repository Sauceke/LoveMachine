using HarmonyLib;
using LoveMachine.Core;
using System;
using UnityEngine;

namespace LoveMachine.SCS
{
    internal static class Hooks
    {
        public static void InstallHooks()
        {
            var hScene = Type.GetType("H_Scene, Assembly-CSharp");
            var startH = new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(StartH)));
            var endH = new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(EndH)));
            var harmony = new Harmony(typeof(Hooks).FullName);
            harmony.Patch(AccessTools.Method(hScene, "ChangeStyle"), postfix: startH);
            harmony.Patch(AccessTools.Method(hScene, "EndScene"), prefix: endH);
        }

        public static void StartH(MonoBehaviour __instance) =>
            CoreConfig.ManagerObject.GetComponent<SecrossphereGame>().StartH(__instance);

        public static void EndH() =>
            CoreConfig.ManagerObject.GetComponent<SecrossphereGame>().EndH();
    }
}