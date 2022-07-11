using System.IO;
using BepInEx;
using BepInEx.Logging;

namespace LoveMachine.Core
{
    public static class CoreConfig
    {
        public const string PluginName = "LoveMachine";
        public const string GUID = "Sauceke.LoveMachine";
        public const string Version = VersionInfo.Version;

        public static ManualLogSource Logger { get; private set; }
        public static string PluginDirectoryPath { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin, ManualLogSource logger,
            string girlMappingHeader, string[] girlMappingOptions)
        {
            Logger = logger;
            PluginDirectoryPath = Path.GetDirectoryName(plugin.Info.Location)
                .TrimEnd(Path.DirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            ButtplugConfig.Initialize(plugin);
            DeviceListConfig.Initialize(plugin, girlMappingHeader, girlMappingOptions);
            KillSwitchConfig.Initialize(plugin);
            RotatorConfig.Initialize(plugin);
            StrokerConfig.Initialize(plugin);
            VibratorConfig.Initialize(plugin);
        }
    }
}
