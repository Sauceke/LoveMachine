using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var endProc = new HarmonyMethod(AccessTools.Method(typeof(HSceneTriggers),
                nameof(HSceneTriggers.EndProc)));
            var harmony = new Harmony(typeof(Hooks).FullName);
            harmony.Patch(AccessTools.Method(sexManager, "Start"), postfix: start);
            harmony.Patch(AccessTools.Method(sexManager, "EndProc"), postfix: endProc);
        }

        private static class HSceneTriggers
        {
            public static void Start()
            {
                CoreConfig.Logger.LogDebug("H Scene started.");
                CoreConfig.ManagerObject.GetComponent<RoomGirlGame>().StartH();
            }

            public static void EndProc()
            {
                CoreConfig.Logger.LogDebug("H Scene ended.");
                CoreConfig.ManagerObject.GetComponent<RoomGirlGame>().EndH();
            }
        }
    }
}
