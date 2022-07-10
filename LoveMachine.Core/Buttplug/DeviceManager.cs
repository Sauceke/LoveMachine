using System.Collections.Generic;
using LitJson;

namespace LoveMachine.Core
{
    internal class DeviceManager
    {
        public static void SaveDeviceSettings(List<Device> devices)
        {
            if (!CoreConfig.SaveDeviceSettings.Value)
            {
                CoreConfig.DeviceSettingsJson.Value = "[]";
                return;
            }
            var settings =
                JsonMapper.ToObject<List<DeviceSettings>>(CoreConfig.DeviceSettingsJson.Value);
            var devicesCopy = new List<Device>(devices);
            devices = null;
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
            CoreConfig.DeviceSettingsJson.Value = JsonMapper.ToJson(settings);
        }

        public static void LoadDeviceSettings(List<Device> devices)
        {
            if (!CoreConfig.SaveDeviceSettings.Value)
            {
                return;
            }
            var settings =
                JsonMapper.ToObject<List<DeviceSettings>>(CoreConfig.DeviceSettingsJson.Value);
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
