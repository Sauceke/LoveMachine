using BepInEx;
using BepInEx.Configuration;

namespace LoveMachine.Core
{
    public static class ButtplugConfig
    {
        public static ConfigEntry<string> WebSocketAddress { get; private set; }
        public static ConfigEntry<int> LatencyMs { get; private set; }
        public static ConfigEntry<int> UpdateFrequency { get; private set; }

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
                    acceptableValues: null,
                    new ConfigurationManagerAttributes { Order = order-- }));
            LatencyMs = plugin.Config.Bind(
                section: intifaceSettingsTitle,
                key: "Latency (ms)",
                defaultValue: 0,
                new ConfigDescription(
                    "The difference in latency between your toy and your display.\n" +
                    "Negative if your toy has lower latency than your display.",
                    new AcceptableValueRange<int>(-500, 500),
                    new ConfigurationManagerAttributes { Order = order-- }));
            UpdateFrequency = plugin.Config.Bind(
                section: intifaceSettingsTitle,
                key: "Update Frequency (per second)",
                defaultValue: 10,
                new ConfigDescription(
                    "Maximum device commands per second.",
                    new AcceptableValueRange<int>(1, 30),
                    new ConfigurationManagerAttributes { Order = order-- }));
        }
    }
}
