using System.Collections.Generic;
using System.Linq;
using LitJson;
using UnityEngine;

namespace LoveMachine.Core
{
    internal class DeviceManager : MonoBehaviour
    {
        private ButtplugWsClient client;

        private void Start()
        {
            client = gameObject.GetComponent<ButtplugWsClient>();
            client.OnDeviceListUpdated += ReloadDeviceSettings;
        }

        private void OnDestroy() => SaveDeviceSettings(client.Devices, exiting: true);

        private void ReloadDeviceSettings(object sender, DeviceListEventArgs args)
        {
            SaveDeviceSettings(args.Before);
            LoadDeviceSettings(args.After);
        }

        private void SaveDeviceSettings(List<Device> devices, bool exiting = false)
        {
            var settings = JsonMapper.ToObject<List<DeviceSettings>>(
                DeviceListConfig.DeviceSettingsJson.Value);
            devices.ForEach(device =>
                settings.Remove(settings.Find(setting =>
                    setting.DeviceName == device.DeviceName)));
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
            DeviceListConfig.DeviceSettingsJson.Value = JsonMapper.ToJson(settings);
        }

        private void LoadDeviceSettings(List<Device> devices)
        {
            var settings = JsonMapper.ToObject<List<DeviceSettings>>(
                DeviceListConfig.DeviceSettingsJson.Value);
            foreach (var device in devices)
            {
                device.Settings = settings
                    .Find(setting => device.DeviceName == setting.DeviceName)
                    ?? device.Settings;
                settings.Remove(device.Settings);
            }
        }
    }
}
