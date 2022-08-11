using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using LitJson;
using UnityEngine;

namespace LoveMachine.Core
{
    public static class DeviceListConfig
    {
        private static List<Device> cachedDeviceList = new List<Device>();
        private static ConfigEntry<string> deviceSettingsJson;
        private static ConfigEntry<bool> showOfflineDevices;

        public static ConfigEntry<bool> SaveDeviceMapping { get; private set; }

        public static List<DeviceSettings> DeviceSettings
        {
            get => JsonMapper.ToObject<List<DeviceSettings>>(deviceSettingsJson.Value);
            set => deviceSettingsJson.Value = JsonMapper.ToJson(value);
        }
        
        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            const string deviceListTitle = "Device List";
            SaveDeviceMapping = plugin.Config.Bind(
                section: deviceListTitle,
                key: "Save device assignments",
                defaultValue: false,
                new ConfigDescription("",
                    tags: new ConfigurationManagerAttributes { Order = order -- }));
            showOfflineDevices = plugin.Config.Bind(
                section: deviceListTitle,
                key: "Show offline devices",
                defaultValue: false,
                new ConfigDescription("",
                    tags: new ConfigurationManagerAttributes { Order = order-- }));
            deviceSettingsJson = plugin.Config.Bind(
                section: deviceListTitle,
                key: "Devices",
                defaultValue: "[]",
                new ConfigDescription("",
                    tags: new ConfigurationManagerAttributes
                    {
                        CustomDrawer = DeviceListDrawer,
                        HideSettingName = true,
                        HideDefaultButton = true,
                        Order = order--
                    }));
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
                    if (serverController.IsConnected
                        && GUILayout.Button("Scan", GUILayout.Width(150)))
                    {
                        serverController.StartScan();
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
                if (showOfflineDevices.Value)
                {
                    DrawOfflineDeviceList(cachedDeviceList);
                }
            }
            GUILayout.EndVertical();
        }

        private static void DrawDevicePanel(Device device)
        {
            GUILayout.BeginVertical(GetDevicePanelStyle());
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

        private static void DrawOfflineDeviceList(List<Device> onlineDevices)
        {
            var settings = DeviceSettings;
            foreach (var setting in settings)
            {
                if (onlineDevices.Any(device => device.Matches(setting)))
                {
                    continue;
                }
                GUILayout.BeginVertical(GetOfflineDevicePanelStyle());
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label($"{setting.DeviceName} (Offline)");
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    GUIUtil.SingleSpace();
                    setting.Draw();
                }
                GUILayout.EndVertical();
                GUIUtil.SingleSpace();
            }
            DeviceSettings = settings;
        }

        private static void TestDevice(Device device)
        {
            ButtplugController.Test<StrokerController>(device);
            ButtplugController.Test<VibratorController>(device);
            ButtplugController.Test<RotatorController>(device);
        }

        private static GUIStyle GetDevicePanelStyle() => new GUIStyle(GUI.skin.box)
        {
            margin = new RectOffset(left: 20, right: 20, top: 0, bottom: 0),
            normal = new GUIStyleState { background = GetTexture(new Color(0f, 1f, 0.5f, 0.2f)) }
        };

        private static GUIStyle GetOfflineDevicePanelStyle() => new GUIStyle(GetDevicePanelStyle())
        {
            normal = new GUIStyleState { background = GetTexture(new Color(1f, 0f, 0.2f, 0.2f)) }
        };

        private static Texture2D GetTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixels(new Color[] { color });
            texture.Apply();
            return texture;
        }
    }
}
