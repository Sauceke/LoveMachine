using BepInEx.Bootstrap;
using BepInEx.Logging;
using UnityEngine;

namespace LoveMachine.Core.PlatformSpecific
{
    public static class Globals
    {
        public const string PluginName = "LoveMachine";
        public const string GUID = "Sauceke.LoveMachine";
        public const string Version = VersionInfo.Version;

        public static ManualLogSource Logger { get; private set; }
        public static GameObject ManagerObject => Chainloader.ManagerObject;

        internal static void Initialize(ManualLogSource logger) => Logger = logger;
    }
}