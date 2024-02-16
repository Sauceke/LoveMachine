using BepInEx;
using BepInEx.Configuration;
using LoveMachine.Core.NonPortable;

namespace LoveMachine.Core.Config
{
    internal static class ButtplugConfig
    {
        public static ConfigEntry<string> WebSocketHost { get; private set; }
        public static ConfigEntry<int> WebSocketPort { get; private set; }
        public static ConfigEntry<int> ReconnectBackoffSecs { get; private set; }

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
            ReconnectBackoffSecs = plugin.Config.Bind(
                section: intifaceSettingsTitle,
                key: "Reconnect Backoff Time (seconds)",
                defaultValue: 20,
                new ConfigDescription(
                    "Wait this long before reconnecting after a failed connection or disconnect.",
                    acceptableValues: new AcceptableValueRange<int>(1, 60),
                    tags: new ConfigurationManagerAttributes { Order = --order }));
        }
    }
}