using System;
using BepInEx;
using BepInEx.Configuration;

namespace LoveMachine.Core
{
    internal static class DeviceListConfig
    {
        public static ConfigEntry<bool> SaveDeviceMapping { get; private set; }
        public static ConfigEntry<bool> ShowOfflineDevices { get; private set; }
        public static ConfigEntry<string> DeviceSettingsJson { get; private set; }

        public static event EventHandler<EventArgs> OnDraw; 

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            const string deviceListTitle = "Device List";
            SaveDeviceMapping = plugin.Config.Bind(
                section: deviceListTitle,
                key: "Save Device Assignments",
                defaultValue: false,
                new ConfigDescription("",
                    tags: new ConfigurationManagerAttributes { Order = --order }));
            ShowOfflineDevices = plugin.Config.Bind(
                section: deviceListTitle,
                key: "Show Offline Devices",
                defaultValue: false,
                new ConfigDescription("",
                    tags: new ConfigurationManagerAttributes { Order = --order }));
            DeviceSettingsJson = plugin.Config.Bind(
                section: deviceListTitle,
                key: "Devices",
                defaultValue: "[]",
                new ConfigDescription("",
                    tags: new ConfigurationManagerAttributes
                    {
                        CustomDrawer = entry => OnDraw.Invoke(null, EventArgs.Empty),
                        HideSettingName = true,
                        HideDefaultButton = true,
                        Order = --order
                    }));
        }
    }
}