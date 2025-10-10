using System.Collections.Generic;
using System.Linq;
using LitJson;
using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.Config;
using UnityEngine;

namespace LoveMachine.Core.Buttplug
{
    internal class DeviceManager : MonoBehaviour
    {
        private ButtplugWsClient client;

        public static List<DeviceSettings> DeviceSettings
        {
            get => JsonMapper.ToObject<List<DeviceSettings>>(
                DeviceListConfig.DeviceSettingsJson.Value);
            set => DeviceListConfig.DeviceSettingsJson.Value = JsonMapper.ToJson(value);
        }

        private void Start()
        {
            client = GetComponent<ButtplugWsClient>();
            client.OnDeviceListUpdated += ReloadDeviceSettings;
        }

        private void OnDestroy() => SaveDeviceSettings(client.Devices, exiting: true);

        private static void ReloadDeviceSettings(object sender,
            ButtplugWsClient.DeviceListEventArgs args)
        {
            SaveDeviceSettings(args.Before);
            LoadDeviceSettings(args.After);
        }

        private static void SaveDeviceSettings(List<Device> devices, bool exiting = false)
        {
            var settings = DeviceSettings;
            devices.ForEach(device => settings.Remove(settings.Find(device.Matches)));
            settings = devices.Select(device => device.Settings).Concat(settings).ToList();
            if (exiting && !DeviceListConfig.SaveDeviceMapping.Value)
            {
                var defaults = new FeatureSettings();
                foreach (var setting in settings)
                {
                    setting.GlobalFeatureSettings.GirlIndex = defaults.GirlIndex;
                    setting.GlobalFeatureSettings.Bone = defaults.Bone;
                }
            }
            DeviceSettings = settings;
        }

        private static void LoadDeviceSettings(List<Device> devices)
        {
            var settings = DeviceSettings;
            foreach (var device in devices)
            {
                device.Settings = settings.Find(device.Matches) ?? InitDefaults(device);
                settings.Remove(device.Settings);
                device.CleanUpSettings();
            }
        }

        private static DeviceSettings InitDefaults(Device device)
        {
            var settings = device.Settings;
            device.CleanUpSettings();
            switch (device.DeviceName)
            {
                case "The Handy":
                    if (settings.StrokerSettings == null)
                    {
                        break;
                    }
                    settings.StrokerSettings.SmoothStroking = true;
                    break;

                case "Lovense Solace Pro":
                    if (settings.StrokerSettings == null)
                    {
                        break;
                    }
                    settings.UseSeparateFeatureSettings = true;
                    foreach (var scalarSetting in settings.ScalarCmdSettings)
                    {
                        scalarSetting.Enabled = false;
                    }
                    break;
            }
            return settings;
        }
    }
}