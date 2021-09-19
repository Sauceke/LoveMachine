using System;
using System.Collections;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using UnityEngine;

namespace LoveMachine.Core
{
    // BepInEx seems to care only for direct subclasses of BaseUnityPlugin, so making a common
    // plugin base for LoveMachine is not possible.
    // This is ugly but my hands are tied.
    public class PluginInitializer
    {
        private BaseUnityPlugin plugin;
        private string girlMappingHeader;
        private string[] girlMappingOptions;
        private string actionMappingHeader;
        private string[] actionMappingOptions;
        private Type[] controllers;

        public static void Initialize(BaseUnityPlugin plugin,
            string girlMappingHeader, string[] girlMappingOptions,
            string actionMappingHeader, string[] actionMappingOptions,
            params Type[] controllers)
        {
            new PluginInitializer
            {
                plugin = plugin,
                girlMappingHeader = girlMappingHeader,
                girlMappingOptions = girlMappingOptions,
                actionMappingHeader = actionMappingHeader,
                actionMappingOptions = actionMappingOptions,
                controllers = controllers
            }
            .Start();
        }

        private void Start()
        {
            InitSettings();
            Chainloader.ManagerObject.AddComponent<ButtplugWsClient>();
            foreach (var controller in controllers)
            {
                Chainloader.ManagerObject.AddComponent(controller);
            }
        }

        private void InitSettings()
        {
            //
            // Network settings
            //
            CoreConfig.WebSocketAddress = plugin.Config.Bind(
                section: "Network Settings",
                key: "WebSocket address",
                defaultValue: "ws://localhost:12345/",
                "The Buttplug server address (requires game restart).");
            //
            // Stroker settings
            //
            string strokerSettingsTitle = "Stroker Settings";
            CoreConfig.MaxStrokesPerMinute = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Maximum strokes per minute",
                defaultValue: 140,
                new ConfigDescription(
                    "The top speed possible on your stroker in your preferred Fast Stroke Zone.\n" +
                    "LoveMachine will slow down animations if necessary based on this value.",
                    new AcceptableValueRange<int>(0, 300),
                    new ConfigurationManagerAttributes { Order = 10 }));
            CoreConfig.LatencyMs = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Latency (ms)",
                defaultValue: 0,
                new ConfigDescription(
                    "The difference in latency between your stroker and your display.\n" +
                    "Negative if your stroker has lower latency than your display.",
                    new AcceptableValueRange<int>(-500, 500),
                    new ConfigurationManagerAttributes { Order = 9 }));
            CoreConfig.SlowStrokeZoneMin = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Slow Stroke Zone Min",
                defaultValue: 0,
                new ConfigDescription(
                    "Lowest position the stroker will move to when going slow.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes { Order = 8 }));
            CoreConfig.SlowStrokeZoneMax = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Slow Stroke Zone Max",
                defaultValue: 100,
                new ConfigDescription(
                    "Highest position the stroker will move to when going slow.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes { Order = 7 }));
            CoreConfig.FastStrokeZoneMin = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Fast Stroke Zone Min",
                defaultValue: 10,
                new ConfigDescription(
                    "Lowest position the stroker will move to when going fast.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes { Order = 6 }));
            CoreConfig.FastStrokeZoneMax = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Fast Stroke Zone Max",
                defaultValue: 80,
                new ConfigDescription(
                    "Highest position the stroker will move to when going fast.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes { Order = 5 }));
            //
            // Vibrator settings
            //
            string vibrationSettingsTitle = "Vibration Settings";
            CoreConfig.EnableVibrate = plugin.Config.Bind(
                section: vibrationSettingsTitle,
                key: "Enable Vibrators",
                defaultValue: ButtplugController.VibrationMode.Both,
                "Maps control speed to vibrations");
            CoreConfig.SyncVibrationWithAnimation = plugin.Config.Bind(
                section: vibrationSettingsTitle,
                key: "Vibration With Animation",
                defaultValue: true,
                "Maps vibrations to a wave pattern in sync with animations.");
            CoreConfig.VibrationUpdateFrequency = plugin.Config.Bind(
                section: vibrationSettingsTitle,
                key: "Update Frequency (per second)",
                defaultValue: 30,
                "Average times per second we update the vibration state.");
            //
            // Kill switch settings
            //
            string killSwitchSettingsTitle = "Kill Switch Settings";
            CoreConfig.KillSwitch = plugin.Config.Bind(
                section: killSwitchSettingsTitle,
                key: "Emergency Stop Key Binding",
                defaultValue: new KeyboardShortcut(KeyCode.Space),
                "Shortcut to stop all devices immediately.");
            CoreConfig.ResumeSwitch = plugin.Config.Bind(
                section: killSwitchSettingsTitle,
                key: "Resume Key Binding",
                defaultValue: new KeyboardShortcut(KeyCode.F8),
                "Shortcut to resume device activities.");
            //
            // Device list settings
            //
            string deviceListTitle = "Device List";
            CoreConfig.DeviceSettingsJson = plugin.Config.Bind(
                section: deviceListTitle,
                key: "Connected",
                defaultValue: "[]",
                new ConfigDescription(
                    "",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        CustomDrawer = DeviceListDrawer,
                        HideSettingName = true,
                        HideDefaultButton = true
                    }
                )
            );
            CoreConfig.SaveDeviceSettings = plugin.Config.Bind(
                section: deviceListTitle,
                key: "Save device assignments",
                defaultValue: false);
        }

        private void DeviceListDrawer(ConfigEntryBase entry)
        {
            var serverController = Chainloader.ManagerObject.GetComponent<ButtplugWsClient>();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Connect", GUILayout.Width(150)))
                    {
                        serverController.Connect();
                    }
                    if (serverController.IsConnected)
                    {
                        if (GUILayout.Button("Scan", GUILayout.Width(150)))
                        {
                            serverController.StartScan();
                        }
                    }
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(12);

                // table header
                float totalWidth = GUILayoutUtility.GetWindowsBounds().width * .9f;
                int columns = 6;
                float columnWidth = totalWidth / columns;
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                {
                    GUILayout.Label("Device Name", GUILayout.Width(columnWidth));
                    GUILayout.Label("Stroker", GUILayout.Width(columnWidth));
                    GUILayout.Label("Vibrators", GUILayout.Width(columnWidth));
                    if (girlMappingHeader != null)
                    {
                        GUILayout.Label(girlMappingHeader, GUILayout.Width(columnWidth));
                    }
                    if (actionMappingHeader != null)
                    {
                        GUILayout.Label(actionMappingHeader, GUILayout.Width(columnWidth));
                    }
                    GUILayout.Label("Test Device", GUILayout.Width(columnWidth));
                }
                GUILayout.EndHorizontal();

                foreach (var device in serverController.Devices)
                {
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    {
                        GUILayout.Label(device.DeviceName, GUILayout.Width(columnWidth));
                        GUILayout.Toggle(device.IsStroker, "", GUILayout.Width(columnWidth));
                        GUILayout.Toggle(device.IsVibrator, "", GUILayout.Width(columnWidth));
                        if (girlMappingOptions != null)
                        {
                            device.GirlIndex = GUILayout.SelectionGrid(
                                selected: device.GirlIndex,
                                girlMappingOptions,
                                xCount: 1,
                                GUILayout.Width(columnWidth));
                        }
                        if (actionMappingOptions != null)
                        {
                            device.ActionIndex = GUILayout.SelectionGrid(
                                selected: device.ActionIndex,
                                actionMappingOptions,
                                xCount: 1,
                                GUILayout.Width(columnWidth));
                        }
                        GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                        {
                            if (GUILayout.Button("Test Slow"))
                            {
                                TestStrokerAsync(device, false);
                            }
                            if (GUILayout.Button("Test Fast"))
                            {
                                TestStrokerAsync(device, true);
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        private void TestStrokerAsync(Device device, bool fast)
        {
            var controller = Chainloader.ManagerObject.GetComponents<ButtplugController>()[0];
            controller.HandleCoroutine(TestStroker(device, fast));
        }

        private IEnumerator TestStroker(Device device, bool fast)
        {
            var controller = Chainloader.ManagerObject.GetComponents<ButtplugController>()[0];
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
