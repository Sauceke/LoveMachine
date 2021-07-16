using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace KK_ButtPlugin
{
    [BepInPlugin(GUID, "ButtPlugin", Version)]
    internal class ButtPlugin : BaseUnityPlugin
    {
        public const string GUID = "Sauceke.ButtPlugin";
        public const string Version = "1.0.0";

        public static new ManualLogSource Logger { get; private set; }

        public static ConfigEntry<string> WebSocketAddress { get; private set; }
        public static ConfigEntry<int> MaxStrokesPerMinute { get; private set; }
        public static ConfigEntry<int> LatencyMs { get; private set; }
        public static ConfigEntry<bool> EnableVibrate { get; private set; }

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
                "The top speed possible on your stroker at 70% stroke length.");
            LatencyMs = Config.Bind(
                section: "Device",
                key: "Latency (ms)",
                defaultValue: 0,
                "The difference in latency between your stroker and your display. \n" +
                "Negative if your stroker has lower latency.");
            EnableVibrate = Config.Bind(
                section: "Device",
                key: "Enable Vibrators",
                defaultValue: true,
                "Maps control speed to vibrations");
            Config.Bind(
                section: "Device List",
                key: "Connected",
                defaultValue: "",
                new ConfigDescription(
                    "",
                    null,
                    new ConfigurationManagerAttributes { 
                        CustomDrawer = DeviceListDrawer,
                        HideSettingName = true,
                        HideDefaultButton = true,
                        Order = 999  // show device list at the end
                    }
                )
            );
            Logger = base.Logger;
            Chainloader.ManagerObject.AddComponent<ButtplugController>();
            Hooks.InstallHooks();
        }

        static void DeviceListDrawer(ConfigEntryBase entry)
        {
            var controller = Chainloader.ManagerObject.GetComponent<ButtplugController>();
            
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Connect", GUILayout.Width(150)))
                    {
                        controller.Connect();
                    }
                    if (controller.IsConnected)
                    {
                        if (GUILayout.Button("Scan", GUILayout.Width(150)))
                        {
                            controller.Scan();
                        }
                    }
                    GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(12);

                // table header
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    GUILayout.Label("Device Name", GUILayout.ExpandWidth(true));
                    GUILayout.Label("Stroker", GUILayout.Width(100));
                    GUILayout.Label("Vibrators", GUILayout.Width(100));
                    GUILayout.Label("Threesome Role", GUILayout.Width(100));
            GUILayout.EndHorizontal();
            
                foreach (var device in controller.Devices)
                {
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                        GUILayout.Label(device.DeviceName, GUILayout.ExpandWidth(true));
                        GUILayout.Toggle(device.IsStroker, "", GUILayout.Width(100));
                        GUILayout.Toggle(device.IsVibrator, "", GUILayout.Width(100));
                        var options = new string[] { "First girl", "Second girl", "Off" };
                        device.GirlIndex = GUILayout.SelectionGrid(device.GirlIndex, options, 1);
                    GUILayout.EndHorizontal();
                }
                
            GUILayout.EndVertical();
        }
    }
}
