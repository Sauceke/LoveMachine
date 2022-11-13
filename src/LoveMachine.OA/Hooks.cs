using BepInEx.Bootstrap;
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
            var start = new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(StartH)));
            var harmony = new Harmony(typeof(Hooks).FullName);
            harmony.Patch(AccessTools.Method(sexManager, "InitializeAsync"), postfix: start);
        }

        public static void StartH(MonoBehaviour __instance) =>
            Chainloader.ManagerObject.GetComponent<OurApartmentGame>().StartH(__instance);
    }
}