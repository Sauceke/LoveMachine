using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using UnityEngine;

namespace LoveMachine.Core
{
    public static class DeviceListConfig
    {
        private static readonly string[] ordinals =
            { "First", "Second", "Third", "Fourth", "Fifth", "Sixth" };
        private static readonly string[] boneNames = Enum.GetNames(typeof(Bone))
            .Select(camelCase => Regex.Replace(camelCase, ".([A-Z])", " $1"))
            .ToArray();
        private static List<Device> cachedDeviceList;

        public static ConfigEntry<bool> SaveDeviceSettings { get; private set; }
        public static ConfigEntry<string> DeviceSettingsJson { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            string deviceListTitle = "Device List";
            DeviceSettingsJson = plugin.Config.Bind(
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
            SaveDeviceSettings = plugin.Config.Bind(
                section: deviceListTitle,
                key: "Save device assignments",
                defaultValue: false);
        }

        private static void DeviceListDrawer(ConfigEntryBase entry)
        {
            var serverController = Chainloader.ManagerObject.GetComponent<ButtplugWsClient>();
            var game = Chainloader.ManagerObject.GetComponent<GameDescriptor>();
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
                float totalWidth = Mathf.Min(Screen.width, 650) * .9f;
                int columns = 7;
                float columnWidth = totalWidth / columns;
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                {
                    GUILayout.Label("Device Name", GUILayout.Width(columnWidth));
                    GUILayout.Label("Stroker", GUILayout.Width(columnWidth));
                    GUILayout.Label("Vibrators", GUILayout.Width(columnWidth));
                    GUILayout.Label("Rotators", GUILayout.Width(columnWidth));
                    if (game.MaxHeroineCount > 1)
                    {
                        GUILayout.Label("Group Role", GUILayout.Width(columnWidth));
                    }
                    GUILayout.Label("Body Part", GUILayout.Width(columnWidth));
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
                        GUILayout.Toggle(device.IsRotator, "", GUILayout.Width(columnWidth));
                        if (game.MaxHeroineCount > 1)
                        {
                            string[] girlMappingOptions = Enumerable.Range(0, game.MaxHeroineCount)
                                .Select(index => $"{ordinals[index]} Girl")
                                .Concat(new string[]{ "Off" })
                                .ToArray();
                            device.Settings.GirlIndex = GUILayout.SelectionGrid(
                                selected: device.Settings.GirlIndex,
                                girlMappingOptions,
                                xCount: 1,
                                GUILayout.Width(columnWidth));
                        }
                        device.Settings.Bone = (Bone)GUILayout.SelectionGrid(
                            selected: (int)device.Settings.Bone,
                            boneNames,
                            xCount: 1,
                            GUILayout.Width(columnWidth));
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
                            if (device.IsRotator)
                            {
                                if (GUILayout.Button("Test Slow"))
                                {
                                    TestRotatorAsync(device, fast: false);
                                }
                                if (GUILayout.Button("Test Fast"))
                                {
                                    TestRotatorAsync(device, fast: true);
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

        private static void TestStrokerAsync(Device device, bool fast, bool hard)
        {
            var controller = Chainloader.ManagerObject.GetComponent<StrokerController>();
            controller.HandleCoroutine(TestStroker(device, fast, hard));
        }

        private static IEnumerator TestStroker(Device device, bool fast, bool hard)
        {
            var controller = Chainloader.ManagerObject.GetComponent<StrokerController>();
            float strokeTimeSecs = 60f / StrokerConfig.MaxStrokesPerMinute.Value;
            if (!fast)
            {
                strokeTimeSecs *= 2;
            }
            for (int i = 0; i < 3; i++)
            {
                controller.HandleCoroutine(
                    controller.DoStroke(device.Settings.GirlIndex, device.Settings.Bone,
                        strokeTimeSecs, forceHard: hard));
                yield return new WaitForSecondsRealtime(strokeTimeSecs);
            }
        }

        private static void TestRotatorAsync(Device device, bool fast)
        {
            var controller = Chainloader.ManagerObject.GetComponent<RotatorController>();
            controller.HandleCoroutine(TestRotator(device, fast));
        }

        private static IEnumerator TestRotator(Device device, bool fast)
        {
            var controller = Chainloader.ManagerObject.GetComponent<RotatorController>();
            float strokeTimeSecs = 60f / StrokerConfig.MaxStrokesPerMinute.Value;
            if (!fast)
            {
                strokeTimeSecs *= 2;
            }
            for (int i = 0; i < 3; i++)
            {
                controller.HandleCoroutine(controller.DoRotate(device.Settings.GirlIndex,
                    device.Settings.Bone, true, strokeTimeSecs));
                yield return new WaitForSecondsRealtime(strokeTimeSecs);
            }
            controller.HandleCoroutine(controller.DoRotate(device.Settings.GirlIndex,
                device.Settings.Bone, true, 0));
        }
    }
}
