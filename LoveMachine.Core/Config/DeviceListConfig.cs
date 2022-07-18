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
        private static readonly GUIStyle deviceControlsStyle = new GUIStyle()
        {
            padding = new RectOffset()
            {
                left = 20,
                right = 20
            },
            normal = new GUIStyleState
            {
                background = GetDeviceControlsTexture()
            }
        };
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
                // imgui doesn't expect the layout to change outside of layout events
                if (Event.current.type == EventType.Layout)
                {
                    cachedDeviceList = serverController.Devices;
                }
                foreach (var device in cachedDeviceList)
                {
                    GUILayout.BeginVertical(deviceControlsStyle);
                    {
                        device.Draw();
                    }
                    GUILayout.EndVertical();
                    GUILayout.Space(20);
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
                controller.HandleCoroutine(controller.DoRotate(device, true, strokeTimeSecs));
                yield return new WaitForSecondsRealtime(strokeTimeSecs);
            }
            controller.HandleCoroutine(controller.DoRotate(device, true, 0));
        }

        private static Texture2D GetDeviceControlsTexture()
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixels(new Color[] { new Color(0f, 0f, 0f, 0.3f) });
            texture.Apply();
            return texture;
        }
    }
}
