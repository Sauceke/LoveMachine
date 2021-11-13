using System;
using System.Collections;
using System.Collections.Generic;
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
        private List<Device> cachedDeviceList;

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
            int order = 1000;
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
                    new ConfigurationManagerAttributes { Order = order-- }));
            CoreConfig.LatencyMs = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Latency (ms)",
                defaultValue: 0,
                new ConfigDescription(
                    "The difference in latency between your stroker and your display.\n" +
                    "Negative if your stroker has lower latency than your display.",
                    new AcceptableValueRange<int>(-500, 500),
                    new ConfigurationManagerAttributes { Order = order-- }));
            CoreConfig.SlowStrokeZoneMin = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Slow Stroke Zone Min",
                defaultValue: 0,
                new ConfigDescription(
                    "Lowest position the stroker will move to when going slow.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes
                    {
                        Order = order--,
                        CustomDrawer = entry => { },
                        HideSettingName = true,
                        HideDefaultButton = true
                    }));
            CoreConfig.SlowStrokeZoneMax = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Slow Stroke Zone Max",
                defaultValue: 100,
                new ConfigDescription(
                    "Highest position the stroker will move to when going slow.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes
                    {
                        Order = order--,
                        CustomDrawer = entry => { },
                        HideSettingName = true,
                        HideDefaultButton = true
                    }));
            CoreConfig.FastStrokeZoneMin = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Fast Stroke Zone Min",
                defaultValue: 10,
                new ConfigDescription(
                    "Lowest position the stroker will move to when going fast.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes
                    {
                        Order = order--,
                        CustomDrawer = entry => { },
                        HideSettingName = true,
                        HideDefaultButton = true
                    }));
            CoreConfig.FastStrokeZoneMax = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Fast Stroke Zone Max",
                defaultValue: 80,
                new ConfigDescription(
                    "Highest position the stroker will move to when going fast.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes
                    {
                        Order = order--,
                        CustomDrawer = entry => { },
                        HideSettingName = true,
                        HideDefaultButton = true
                    }));
            plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Stroke Zone (Slow)",
                defaultValue: "Ignore this",
                new ConfigDescription(
                    "Range of stroking movement when going slow",
                    tags: new ConfigurationManagerAttributes
                    {
                        Order = order--,
                        CustomDrawer = SlowStrokeZoneDrawer,
                        HideDefaultButton = true,
                    }));
            plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Stroke Zone (Fast)",
                defaultValue: "Ignore this",
                new ConfigDescription(
                    "Range of stroking movement when going fast",
                    tags: new ConfigurationManagerAttributes
                    {
                        Order = order--,
                        CustomDrawer = FastStrokeZoneDrawer,
                        HideDefaultButton = true
                    }));
            CoreConfig.HardSexIntensity = plugin.Config.Bind(
               section: strokerSettingsTitle,
               key: "Hard Sex Intensity",
               defaultValue: 50,
               new ConfigDescription(
                   "Makes hard sex animations feel hard",
                   new AcceptableValueRange<int>(0, 100),
                   new ConfigurationManagerAttributes { Order = order-- }));
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
                defaultValue: 10,
                "Average times per second we update the vibration state.");
            plugin.Config.Bind(
                section: vibrationSettingsTitle,
                key: "Vibration Intensity Range",
                defaultValue: "Ignore this",
                new ConfigDescription(
                    "Vibration intensity will oscillate between these two values.",
                    tags: new ConfigurationManagerAttributes
                    {
                        CustomDrawer = VibrationIntensityDrawer,
                        HideDefaultButton = true,
                    }));
            CoreConfig.VibrationIntensityMin = plugin.Config.Bind(
                section: vibrationSettingsTitle,
                key: "Vibration Intensity Min",
                defaultValue: 0,
                new ConfigDescription(
                    "Lowest vibration intensity.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes
                    {
                        CustomDrawer = entry => { },
                        HideSettingName = true,
                        HideDefaultButton = true
                    }));
            CoreConfig.VibrationIntensityMax = plugin.Config.Bind(
                section: vibrationSettingsTitle,
                key: "Vibration Intensity Max",
                defaultValue: 100,
                new ConfigDescription(
                    "Highest vibration intensity.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes
                    {
                        CustomDrawer = entry => { },
                        HideSettingName = true,
                        HideDefaultButton = true
                    }));
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
                    tags: new ConfigurationManagerAttributes
                    {
                        CustomDrawer = DeviceListDrawer,
                        HideSettingName = true,
                        HideDefaultButton = true
                    }));
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

                // imgui doesn't expect the layout to change outside of layout events
                if (Event.current.type == EventType.Layout)
                {
                    cachedDeviceList = serverController.Devices;
                }

                foreach (var device in cachedDeviceList)
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
                            device.BoneIndex = GUILayout.SelectionGrid(
                                selected: device.BoneIndex,
                                actionMappingOptions,
                                xCount: 1,
                                GUILayout.Width(columnWidth));
                        }
                        GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                        {
                            if (device.IsStroker)
                            {
                                if (GUILayout.Button("Test Slow"))
                                {
                                    TestStrokerAsync(device, fast: false, hard: false);
                                }
                                if (GUILayout.Button("Test Fast"))
                                {
                                    TestStrokerAsync(device, fast: true, hard: false);
                                }
                                if (GUILayout.Button("Test Hard"))
                                {
                                    TestStrokerAsync(device, fast: false, hard: true);
                                }
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        private void SlowStrokeZoneDrawer(ConfigEntryBase entry)
        {
            DrawRangeSlider(CoreConfig.SlowStrokeZoneMin, CoreConfig.SlowStrokeZoneMax);
        }

        private void FastStrokeZoneDrawer(ConfigEntryBase entry)
        {
            DrawRangeSlider(CoreConfig.FastStrokeZoneMin, CoreConfig.FastStrokeZoneMax);
        }

        private void VibrationIntensityDrawer(ConfigEntryBase obj)
        {
            DrawRangeSlider(CoreConfig.VibrationIntensityMin, CoreConfig.VibrationIntensityMax);
        }

        private void DrawRangeSlider(ConfigEntry<int> min, ConfigEntry<int> max)
        {
            float labelWidth = GUI.skin.label.CalcSize(new GUIContent("100%")).x;
            GUILayout.BeginHorizontal();
            {
                float lower = min.Value;
                float upper = max.Value;
                GUILayout.Label(lower + "%", GUILayout.Width(labelWidth));
                RangeSlider.Create(ref lower, ref upper, 0, 100);
                GUILayout.Label(upper + "%", GUILayout.Width(labelWidth));
                if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)))
                {
                    lower = (int)min.DefaultValue;
                    upper = (int)max.DefaultValue;
                }
                min.Value = (int)lower;
                max.Value = (int)upper;
            }
            GUILayout.EndHorizontal();
        }

        private void TestStrokerAsync(Device device, bool fast, bool hard)
        {
            var controller = Chainloader.ManagerObject.GetComponents<ButtplugController>()[0];
            controller.HandleCoroutine(TestStroker(device, fast, hard));
        }

        private IEnumerator TestStroker(Device device, bool fast, bool hard)
        {
            var controller = Chainloader.ManagerObject.GetComponents<ButtplugController>()[0];
            float strokeTimeSecs = 60f / CoreConfig.MaxStrokesPerMinute.Value;
            if (!fast)
            {
                strokeTimeSecs *= 2;
            }
            for (int i = 0; i < 3; i++)
            {
                controller.HandleCoroutine(
                    controller.DoStroke(device.GirlIndex, device.BoneIndex, strokeTimeSecs, hard));
                yield return new WaitForSecondsRealtime(strokeTimeSecs);
            }
        }
    }
}
