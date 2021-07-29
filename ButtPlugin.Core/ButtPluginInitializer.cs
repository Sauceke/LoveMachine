using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using UnityEngine;

namespace ButtPlugin.Core
{
    // BepInEx seems to care only for direct subclasses of BaseUnityPlugin, so making a common
    // plugin base for ButtPlugin is not possible.
    // This is ugly but my hands are tied.
    public class ButtPluginInitializer<S, V>
        where S: ButtplugController
        where V: ButtplugController
    {
        public static void Start(BaseUnityPlugin plugin)
        {
            CoreConfig.WebSocketAddress = plugin.Config.Bind(
                section: "Network",
                key: "WebSocket address",
                defaultValue: "ws://localhost:12345/",
                "The Buttplug server address (requires game restart).");
            CoreConfig.MaxStrokesPerMinute = plugin.Config.Bind(
                section: "Stroker Settings",
                key: "Maximum strokes per minute",
                defaultValue: 140,
                "The top speed possible on your stroker at 70% stroke length.");
            CoreConfig.LatencyMs = plugin.Config.Bind(
                section: "Stroker Settings",
                key: "Latency (ms)",
                defaultValue: 0,
                "The difference in latency between your stroker and your display. \n" +
                "Negative if your stroker has lower latency.");
            CoreConfig.EnableVibrate = plugin.Config.Bind(
                section: "Vibration Settings",
                key: "Enable Vibrators",
                defaultValue: ButtplugController.VibrationMode.Both,
                "Maps control speed to vibrations"
            );
            CoreConfig.SyncVibrationWithAnimation = plugin.Config.Bind(
                section: "Vibration Settings",
                key: "Vibration With Animation",
                defaultValue: false,
                "Maps vibrations to a wave pattern in sync with animations.\n" +
                "Timings are approximations based on animation length and not precise location of stimulation."
            );
            CoreConfig.VibrationUpdateFrequency = plugin.Config.Bind(
                section: "Vibration Settings",
                key: "Update Frequency (per second)",
                defaultValue: 30,
                "Average times per second we update the vibration state."
            );

            plugin.Config.Bind(
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
            CoreConfig.Info = plugin.Info;
            Chainloader.ManagerObject.AddComponent<ButtplugWsClient>();
            Chainloader.ManagerObject.AddComponent<S>();
            Chainloader.ManagerObject.AddComponent<V>();
        }

        private static void DeviceListDrawer(ConfigEntryBase entry)
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
