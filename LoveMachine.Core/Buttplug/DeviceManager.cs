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
            client.OnDeviceListUpdated += (s, a) => LoadDeviceSettings();
        }

        private void OnDestroy() => SaveDeviceSettings();

        private void SaveDeviceSettings()
        {
            if (!DeviceListConfig.SaveDeviceSettings.Value)
            {
                DeviceListConfig.DeviceSettingsJson.Value = "[]";
                return;
            }
            var settings = JsonMapper.ToObject<List<DeviceSettings>>(
                DeviceListConfig.DeviceSettingsJson.Value);
            var devicesCopy = new List<Device>(client.Devices);
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
            DeviceListConfig.DeviceSettingsJson.Value = JsonMapper.ToJson(settings);
        }

        private void LoadDeviceSettings()
        {
            if (!DeviceListConfig.SaveDeviceSettings.Value)
            {
                return;
            }
            var settings = JsonMapper.ToObject<List<DeviceSettings>>(
                DeviceListConfig.DeviceSettingsJson.Value);
            foreach (var device in client.Devices)
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
