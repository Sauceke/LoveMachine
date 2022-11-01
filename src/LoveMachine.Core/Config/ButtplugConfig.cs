using BepInEx;
using BepInEx.Configuration;

namespace LoveMachine.Core
{
    public static class ButtplugConfig
    {
        public static ConfigEntry<string> IntifaceLocation { get; private set; }
        public static ConfigEntry<string> WebSocketHost { get; private set; }
        public static ConfigEntry<int> WebSocketPort { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            const string intifaceSettingsTitle = "Intiface Settings";
            IntifaceLocation = plugin.Config.Bind(
                section: intifaceSettingsTitle,
                key: "Intiface CLI Location",
                defaultValue: @"%LocalAppData%\IntifaceDesktop\engine\IntifaceCLI.exe",
                new ConfigDescription(
                    "Path to the Intiface CLI executable (requires restart).",
                    tags: new ConfigurationManagerAttributes { Order = order-- }));
            WebSocketHost = plugin.Config.Bind(
                section: intifaceSettingsTitle,
                key: "WebSocket Host",
                defaultValue: "ws://127.0.0.1",
                new ConfigDescription(
                    "The Intiface server host URL.",
                    tags: new ConfigurationManagerAttributes { Order = order-- }));
            WebSocketPort = plugin.Config.Bind(
                section: intifaceSettingsTitle,
                key: "WebSocket Port",
                defaultValue: 12345,
                new ConfigDescription(
                    "The Intiface server port.",
                    tags: new ConfigurationManagerAttributes { Order = order-- }));
        }
    }
}
