using BepInEx.Configuration;
using BepInEx.Logging;

namespace ButtPlugin.Core
{
    public class CoreConfig
    {
        public const string GUID = "Sauceke.ButtPlugin";
        public const string Version = ButtPluginVersionInfo.Version;

        public static ManualLogSource Logger { get; set; }

        public static ConfigEntry<string> WebSocketAddress { get; internal set; }
        public static ConfigEntry<int> MaxStrokesPerMinute { get; internal set; }
        public static ConfigEntry<int> LatencyMs { get; internal set; }
        public static ConfigEntry<int> SlowStrokeZoneMin { get; internal set; }
        public static ConfigEntry<int> SlowStrokeZoneMax { get; internal set; }
        public static ConfigEntry<int> FastStrokeZoneMin { get; internal set; }
        public static ConfigEntry<int> FastStrokeZoneMax { get; internal set; }
        public static ConfigEntry<bool> SaveDeviceSettings { get; internal set; }
        public static ConfigEntry<string> DeviceSettingsJson { get; internal set; }

        public static ConfigEntry<ButtplugController.VibrationMode> EnableVibrate { get; internal set; }
        public static ConfigEntry<bool> SyncVibrationWithAnimation { get; internal set; }
        public static ConfigEntry<int> VibrationUpdateFrequency { get; internal set; }

        public static ConfigEntry<KeyboardShortcut> KillSwitch { get; internal set; }
        public static ConfigEntry<KeyboardShortcut> ResumeSwitch { get; internal set; }
    }
}
