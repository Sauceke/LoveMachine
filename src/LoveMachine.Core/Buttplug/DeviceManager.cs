using System.Collections.Generic;
using System.Linq;
using LitJson;
using LoveMachine.Core.Config;
using LoveMachine.Core.Settings;
using UnityEngine;

namespace LoveMachine.Core.Buttplug
{
    internal class DeviceManager : MonoBehaviour
    {
        private ButtplugWsClient client;

        public static List<DeviceSettings> DeviceSettings
        {
            get => JsonMapper.ToObject<List<DeviceSettings>>(DeviceListConfig.DeviceSettingsJson.Value);
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
                var defaults = new DeviceSettings();
                foreach (var setting in settings)
                {
                    setting.GirlIndex = defaults.GirlIndex;
                    setting.Bone = defaults.Bone;
                }
            }
            DeviceSettings = settings;
        }

        private static void LoadDeviceSettings(List<Device> devices)
        {
            var settings = DeviceSettings;
            foreach (var device in devices)
            {
                device.Settings = settings.Find(device.Matches) ?? device.Settings;
                settings.Remove(device.Settings);
                device.CleanUpSettings();
            }
        }
    }
}