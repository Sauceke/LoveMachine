using BepInEx;
using BepInEx.Configuration;

namespace LoveMachine.Core.Config
{
    internal static class ButtplugConfig
    {
        public static ConfigEntry<string> WebSocketHost { get; private set; }
        public static ConfigEntry<int> WebSocketPort { get; private set; }
        public static ConfigEntry<bool> RunIntiface { get; private set; }
        public static ConfigEntry<string> IntifaceArgs { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            const string intifaceSettingsTitle = "Intiface Settings";
            WebSocketHost = plugin.Config.Bind(
                section: intifaceSettingsTitle,
                key: "WebSocket Host",
                defaultValue: "ws://127.0.0.1",
                new ConfigDescription(
                    "The Intiface server host URL.",
                    tags: new ConfigurationManagerAttributes { Order = --order }));
            WebSocketPort = plugin.Config.Bind(
                section: intifaceSettingsTitle,
                key: "WebSocket Port",
                defaultValue: 12345,
                new ConfigDescription(
                    "The Intiface server port.",
                    tags: new ConfigurationManagerAttributes { Order = --order }));
            RunIntiface = plugin.Config.Bind(
                section: intifaceSettingsTitle,
                key: "Run Intiface Engine",
                defaultValue: true,
                new ConfigDescription(
                    "Run the Intiface Engine bundled with the plugin on startup.",
                    tags: new ConfigurationManagerAttributes { Order = --order }));
            IntifaceArgs = plugin.Config.Bind(
                section: intifaceSettingsTitle,
                key: "Intiface Engine Arguments",
                defaultValue: "--websocket-port 12345 --use-bluetooth-le --use-serial --use-xinput",
                new ConfigDescription(
                    "Start Intiface Engine with these arguments.",
                    tags: new ConfigurationManagerAttributes { Order = --order }));
        }
    }
}