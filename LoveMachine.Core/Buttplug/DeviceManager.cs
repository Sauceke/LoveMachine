using System.Collections.Generic;
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

        private void OnDestroy() => SaveDeviceSettings(client.Devices);

        private void ReloadDeviceSettings(object sender, DeviceListEventArgs args)
        {
            SaveDeviceSettings(args.Before);
            LoadDeviceSettings(args.After);
        }

        private void SaveDeviceSettings(List<Device> devices)
        {
            var settings = JsonMapper.ToObject<List<DeviceSettings>>(
                DeviceListConfig.DeviceSettingsJson.Value);
            var devicesCopy = new List<Device>(devices);
            for (int i = 0; i < settings.Count; i++)
            {
                var setting = settings[i];
                int matchingDeviceIndex = devicesCopy.FindIndex(
                    device => string.Equals(device.DeviceName, setting.DeviceName));
                if (matchingDeviceIndex != -1)
                {
                    settings[i] = devicesCopy[matchingDeviceIndex].Settings;
                    devicesCopy.RemoveAt(matchingDeviceIndex);
                }
            }
            foreach (var remainingDevice in devicesCopy)
            {
                settings.Add(remainingDevice.Settings);
            }
            if (!DeviceListConfig.SaveDeviceMapping.Value)
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
                int matchingSettingIndex = settings.FindIndex(
                    setting => string.Equals(device.DeviceName, setting.DeviceName));
                if (matchingSettingIndex != -1)
                {
                    device.Settings = settings[matchingSettingIndex];
                    settings.RemoveAt(matchingSettingIndex);
                }
            }
        }
    }
}
