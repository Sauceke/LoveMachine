using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace ButtPlugin.Core
{
    public class CoreConfig
    {
        public const string GUID = "Sauceke.ButtPlugin";
        public const string Version = "2.0.2";

        public static ManualLogSource Logger { get; set; }
        public static PluginInfo Info { get; internal set; }

        public static ConfigEntry<string> WebSocketAddress { get; internal set; }
        public static ConfigEntry<int> MaxStrokesPerMinute { get; internal set; }
        public static ConfigEntry<int> LatencyMs { get; internal set; }
        public static ConfigEntry<int> StrokeZoneMin { get; internal set; }
        public static ConfigEntry<int> StrokeZoneMax { get; internal set; }

        public static ConfigEntry<ButtplugController.VibrationMode> EnableVibrate { get; internal set; }
        public static ConfigEntry<bool> SyncVibrationWithAnimation { get; internal set; }
        public static ConfigEntry<int> VibrationUpdateFrequency { get; internal set; }
    }
}
