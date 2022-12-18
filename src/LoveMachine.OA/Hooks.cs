using HarmonyLib;
using LoveMachine.Core;
using System;
using UnityEngine;

namespace LoveMachine.OA
{
    internal static class Hooks
    {
        public static void InstallHooks()
        {
            var sexManager = Type.GetType("SexSimControl, Assembly-CSharp");
            var startH = new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(StartH)));
            var endH = new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(EndH)));
            var harmony = new Harmony(typeof(Hooks).FullName);
            harmony.Patch(AccessTools.Method(sexManager, "InitializeAsync"), postfix: startH);
            harmony.Patch(AccessTools.Method(sexManager, "RunSexConclusion"), prefix: endH);
        }

        public static void StartH(MonoBehaviour __instance) =>
            CoreConfig.ManagerObject.GetComponent<OurApartmentGame>().StartH(__instance);

        public static void EndH() =>
            CoreConfig.ManagerObject.GetComponent<OurApartmentGame>().EndH();
    }
}