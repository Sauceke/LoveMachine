using System.Collections;
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
                new ConfigDescription(
                    "The top speed possible on your stroker in your preferred Fast Stroke Zone.\n" +
                    "ButtPlugin will slow down animations if necessary based on this value.",
                    new AcceptableValueRange<int>(0, 300),
                    new ConfigurationManagerAttributes { Order = 10 }));
            CoreConfig.LatencyMs = plugin.Config.Bind(
                section: "Stroker Settings",
                key: "Latency (ms)",
                defaultValue: 0,
                new ConfigDescription(
                    "The difference in latency between your stroker and your display.\n" +
                    "Negative if your stroker has lower latency than your display.",
                    new AcceptableValueRange<int>(-500, 500),
                    new ConfigurationManagerAttributes { Order = 9 }));
            CoreConfig.SlowStrokeZoneMin = plugin.Config.Bind(
                section: "Stroker Settings",
                key: "Slow Stroke Zone Min",
                defaultValue: 0,
                new ConfigDescription(
                    "Lowest position the stroker will move to when going slow.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes { Order = 8 }));
            CoreConfig.SlowStrokeZoneMax = plugin.Config.Bind(
                section: "Stroker Settings",
                key: "Slow Stroke Zone Max",
                defaultValue: 100,
                new ConfigDescription(
                    "Highest position the stroker will move to when going slow.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes { Order = 7 }));
            CoreConfig.FastStrokeZoneMin = plugin.Config.Bind(
                section: "Stroker Settings",
                key: "Fast Stroke Zone Min",
                defaultValue: 10,
                new ConfigDescription(
                    "Lowest position the stroker will move to when going fast.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes { Order = 6 }));
            CoreConfig.FastStrokeZoneMax = plugin.Config.Bind(
                section: "Stroker Settings",
                key: "Fast Stroke Zone Max",
                defaultValue: 80,
                new ConfigDescription(
                    "Highest position the stroker will move to when going fast.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes { Order = 5 }));
            CoreConfig.EnableVibrate = plugin.Config.Bind(
                section: "Vibration Settings",
                key: "Enable Vibrators",
                defaultValue: ButtplugController.VibrationMode.Both,
                "Maps control speed to vibrations");
            CoreConfig.SyncVibrationWithAnimation = plugin.Config.Bind(
                section: "Vibration Settings",
                key: "Vibration With Animation",
                defaultValue: true,
                "Maps vibrations to a wave pattern in sync with animations.\n" +
                "Timings are approximations based on animation length and not precise location of stimulation.");
            CoreConfig.VibrationUpdateFrequency = plugin.Config.Bind(
                section: "Vibration Settings",
                key: "Update Frequency (per second)",
                defaultValue: 30,
                "Average times per second we update the vibration state.");
            CoreConfig.KillSwitch = plugin.Config.Bind(
                section: "Kill Switch",
                key: "Emergency Stop Key Binding",
                defaultValue: new KeyboardShortcut(KeyCode.Space),
                "Shortcut to stop all devices immediately.");
            CoreConfig.ResumeSwitch = plugin.Config.Bind(
                section: "Kill Switch",
                key: "Resume Key Binding",
                defaultValue: new KeyboardShortcut(KeyCode.F8),
                "Shortcut to resume device activities.");
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
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Test Device", GUILayout.Width(100));
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
                        GUILayout.FlexibleSpace();
                        GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                            if (GUILayout.Button("Test Slow"))
                            {
                                TestStrokerAsync(device, false);
                            }
                            if (GUILayout.Button("Test Fast"))
                            {
                                TestStrokerAsync(device, true);
                            }
                        GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
                
            GUILayout.EndVertical();
        }

        private static void TestStrokerAsync(Device device, bool fast)
        {
            var controller = Chainloader.ManagerObject.GetComponent<S>();
            controller.HandleCoroutine(TestStroker(device, fast));
        }

        private static IEnumerator TestStroker(Device device, bool fast)
        {
            var controller = Chainloader.ManagerObject.GetComponent<S>();
            float strokeTimeSecs = 60f / CoreConfig.MaxStrokesPerMinute.Value;
            if (!fast)
            {
                strokeTimeSecs *= 2;
            }
            for (int i = 0; i < 3; i++)
            {
                yield return controller.HandleCoroutine(
                    controller.DoStroke(strokeTimeSecs, device.GirlIndex));
                yield return new WaitForSeconds(strokeTimeSecs / 2);
            }
        }
    }
}
