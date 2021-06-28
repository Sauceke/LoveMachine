using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace KK_ButtPlugin
{
    [BepInPlugin(GUID, "ButtPlugin", Version)]
    internal class ButtPlugin : BaseUnityPlugin
    {
        public const string GUID = "Sauceke.ButtPlugin";
        public const string Version = "1.0.0";

        public static new ManualLogSource Logger { get; private set; }
        public static ButtplugController Controller { get; private set; }

        public static ConfigEntry<string> WebSocketAddress { get; private set; }
        public static ConfigEntry<int> MaxStrokesPerMinute { get; private set; }
        public static ConfigEntry<float> ClimaxSpeed { get; private set; }

        private void Start()
        {
            WebSocketAddress = Config.Bind(
                section: "Network",
                key: "WebSocket address",
                defaultValue: "ws://localhost:12345/",
                "The Buttplug server address (requires game restart).");
            MaxStrokesPerMinute = Config.Bind(
                section: "Device",
                key: "Maximum strokes per minute",
                defaultValue: 140,
                "The top speed possible on your device at 70% stroke length.");
            Logger = base.Logger;
            Controller = new ButtplugController();
            Hooks.InstallHooks();
        }

        private void OnApplicationQuit()
        {
            Controller.OnAppQuit();
        }
    }
}
