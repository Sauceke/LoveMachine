using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using System.IO;
using UnityEngine;

namespace LoveMachine.Core
{
    public static class CoreConfig
    {
        public const string PluginName = "LoveMachine";
        public const string GUID = "Sauceke.LoveMachine";
        public const string Version = VersionInfo.Version;

        public static ManualLogSource Logger { get; private set; }
        public static string PluginDirectoryPath { get; private set; }
        public static GameObject ManagerObject => Chainloader.ManagerObject;

        internal static void Initialize(BaseUnityPlugin plugin, ManualLogSource logger)
        {
            Logger = logger;
            PluginDirectoryPath = Path.GetDirectoryName(plugin.Info.Location)
                .TrimEnd(Path.DirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
        }
    }
}