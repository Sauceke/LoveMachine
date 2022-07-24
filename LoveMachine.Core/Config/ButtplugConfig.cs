using BepInEx;
using BepInEx.Configuration;

namespace LoveMachine.Core
{
    public static class ButtplugConfig
    {
        public static ConfigEntry<string> WebSocketAddress { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            string intifaceSettingsTitle = "Intiface Settings";
            WebSocketAddress = plugin.Config.Bind(
                section: intifaceSettingsTitle,
                key: "WebSocket address",
                defaultValue: "ws://localhost:12345/",
                new ConfigDescription(
                    "The Intiface server address (requires game restart).",
                    tags: new ConfigurationManagerAttributes { Order = order-- }));
        }
    }
}
