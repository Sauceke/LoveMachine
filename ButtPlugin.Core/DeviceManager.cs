using System.Collections.Generic;
using LitJson;

namespace ButtPlugin.Core
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
            List<DeviceSettings> settings =
                JsonMapper.ToObject<List<DeviceSettings>>(CoreConfig.DeviceSettingsJson.Value);
            List<Device> devicesCopy = new List<Device>(devices);
            devices = null;
            for (int i = 0; i < settings.Count; i++)
            {
                var setting = settings[i];
                var matchingDeviceIndex = devicesCopy.FindIndex(
                    device => string.Equals(device.DeviceName, setting.DeviceName));
                if (matchingDeviceIndex != -1)
                {
                    settings[i] = new DeviceSettings(devicesCopy[matchingDeviceIndex]);
                    devicesCopy.RemoveAt(matchingDeviceIndex);
                }
            }
            foreach (var remainingDevice in devicesCopy)
            {
                settings.Add(new DeviceSettings(remainingDevice));
            }
            CoreConfig.DeviceSettingsJson.Value = JsonMapper.ToJson(settings);
        }

        public static void LoadDeviceSettings(List<Device> devices)
        {
            if (!CoreConfig.SaveDeviceSettings.Value)
            {
                return;
            }
            List<DeviceSettings> settings =
                JsonMapper.ToObject<List<DeviceSettings>>(CoreConfig.DeviceSettingsJson.Value);
            foreach (var device in devices)
            {
                var matchingSettingIndex = settings.FindIndex(
                    setting => string.Equals(device.DeviceName, setting.DeviceName));
                if (matchingSettingIndex != -1)
                {
                    settings[matchingSettingIndex].Apply(device);
                    settings.RemoveAt(matchingSettingIndex);
                }
            }
        }
    }
}
