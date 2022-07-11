using BepInEx;
using BepInEx.Configuration;

namespace LoveMachine.Core
{
    public static class ButtplugConfig
    {
        public static ConfigEntry<string> WebSocketAddress { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin) => WebSocketAddress = plugin.Config.Bind(
                section: "Network Settings",
                key: "WebSocket address",
                defaultValue: "ws://localhost:12345/",
                "The Buttplug server address (requires game restart).");
    }
}
