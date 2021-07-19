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
        public const string Version = "1.2.0";

        public static new ManualLogSource Logger { get; private set; }
        public static new PluginInfo Info { get; private set; }

        public static ConfigEntry<string> WebSocketAddress { get; private set; }
        public static ConfigEntry<int> MaxStrokesPerMinute { get; private set; }
        public static ConfigEntry<int> LatencyMs { get; private set; }
        public static ConfigEntry<VibrationMode> EnableVibrate { get; private set; }
        public static ConfigEntry<bool> SyncVibrationWithAnimation { get; private set; }
        public static ConfigEntry<int> VibrationUpdateFrequency { get; private set; }

        private void Start()
        {
            WebSocketAddress = Config.Bind(
                section: "Network",
                key: "WebSocket address",
                defaultValue: "ws://localhost:12345/",
                "The Buttplug server address (requires game restart).");
            MaxStrokesPerMinute = Config.Bind(
                section: "Stroker Settings",
                key: "Maximum strokes per minute",
                defaultValue: 140,
                "The top speed possible on your stroker at 70% stroke length.");
            LatencyMs = Config.Bind(
                section: "Stroker Settings",
                key: "Latency (ms)",
                defaultValue: 0,
                "The difference in latency between your stroker and your display. \n" +
                "Negative if your stroker has lower latency.");
            EnableVibrate = Config.Bind(
                section: "Vibration Settings",
                key: "Enable Vibrators",
                defaultValue: VibrationMode.Both,
                "Maps control speed to vibrations"
            );
            SyncVibrationWithAnimation = Config.Bind(
                section: "Vibration Settings",
                key: "Vibration With Animation",
                defaultValue: false,
                "Maps vibrations to a wave pattern in sync with animations.\n" +
                "Timings are approximations based on animation length and not precise location of stimulation."
            );
            VibrationUpdateFrequency = Config.Bind(
                section: "Vibration Settings",
                key: "Update Frequency (per second)",
                defaultValue: 30,
                "Average times per second we update the vibration state."
            );

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
            Info = base.Info;
            Chainloader.ManagerObject.AddComponent<ButtplugWsClient>();
            Chainloader.ManagerObject.AddComponent<ButtplugStrokerController>();
            Chainloader.ManagerObject.AddComponent<ButtplugVibrationController>();
            Hooks.InstallHooks();
        }

        static void DeviceListDrawer(ConfigEntryBase entry)
        {
            var serverController = Chainloader.ManagerObject.GetComponent<ButtplugWsClient>();
            
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Connect", GUILayout.Width(150)))
                    {
                        serverController.Connect();
                    }
                    if (serverController.IsConnected)
                    {
                        if (GUILayout.Button("Scan", GUILayout.Width(150)))
                        {
                            serverController.Scan();
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
            
                foreach (var device in serverController.Devices)
                {
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                        GUILayout.Label(device.DeviceName, GUILayout.ExpandWidth(true));
                        GUILayout.Toggle(device.IsStroker, "", GUILayout.Width(100));
                        GUILayout.Toggle(device.IsVibrator, "", GUILayout.Width(100));
                        var options = new string[] { "First girl", "Second girl", "Off" };
                        device.GirlIndex = GUILayout.SelectionGrid(device.GirlIndex, options, 1, GUILayout.Width(100));
                    GUILayout.EndHorizontal();
                }
                
            GUILayout.EndVertical();
        }
    }
}
