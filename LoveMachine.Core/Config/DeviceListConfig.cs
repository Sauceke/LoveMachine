using System.Collections.Generic;
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
            margin = new RectOffset(left: 20, top: 0, right: 20, bottom: 0),
            normal = new GUIStyleState
            {
                background = GetDeviceControlsTexture()
            }
        };
        private static List<Device> cachedDeviceList;

        public static ConfigEntry<bool> SaveDeviceMapping { get; private set; }
        public static ConfigEntry<string> DeviceSettingsJson { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            string deviceListTitle = "Device List";
            DeviceSettingsJson = plugin.Config.Bind(
                section: deviceListTitle,
                    key: "Devices",
                    defaultValue: "[]",
                    new ConfigDescription(
                        "",
                        tags: new ConfigurationManagerAttributes
                        {
                            CustomDrawer = DeviceListDrawer,
                            HideSettingName = true,
                            HideDefaultButton = true
                        }));
            SaveDeviceMapping = plugin.Config.Bind(
                section: deviceListTitle,
                key: "Save device assignments",
                defaultValue: false);
        }

        private static void DeviceListDrawer(ConfigEntryBase entry)
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
                GUIUtil.SingleSpace();
                // imgui doesn't expect the layout to change outside of layout events
                if (Event.current.type == EventType.Layout)
                {
                    cachedDeviceList = serverController.Devices;
                }
                foreach (var device in cachedDeviceList)
                {
                    DrawDevicePanel(device);
                }
            }
            GUILayout.EndVertical();
        }

        private static void DrawDevicePanel(Device device)
        {
            GUILayout.BeginVertical(deviceControlsStyle);
            {
                device.Draw();
                GUILayout.BeginHorizontal();
                {
                    GUIUtil.LabelWithTooltip("Test", "Test this device");
                    if (GUILayout.Button("Test"))
                    {
                        TestDevice(device);
                    }
                }
                GUILayout.EndHorizontal();
                GUIUtil.SingleSpace();
            }
            GUILayout.EndVertical();
            GUIUtil.SingleSpace();
        }

        private static void TestDevice(Device device)
        {
            ButtplugController.Test<StrokerController>(device);
            ButtplugController.Test<VibratorController>(device);
            ButtplugController.Test<RotatorController>(device);
        }

        private static Texture2D GetDeviceControlsTexture()
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixels(new Color[] { new Color(0f, 0f, 0f, 0.4f) });
            texture.Apply();
            return texture;
        }
    }
}
