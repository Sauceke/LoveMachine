using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoveMachine.Core
{
    internal class DeviceManager : MonoBehaviour
    {
        private ButtplugWsClient client;

        private void Start()
        {
            client = GetComponent<ButtplugWsClient>();
            client.OnDeviceListUpdated += ReloadDeviceSettings;
        }

        private void OnDestroy() => SaveDeviceSettings(client.Devices, exiting: true);

        private void ReloadDeviceSettings(object sender, ButtplugWsClient.DeviceListEventArgs args)
        {
            SaveDeviceSettings(args.Before);
            LoadDeviceSettings(args.After);
        }

        private static void SaveDeviceSettings(List<Device> devices, bool exiting = false)
        {
            var settings = DeviceListConfig.DeviceSettings;
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
            DeviceListConfig.DeviceSettings = settings;
        }

        private static void LoadDeviceSettings(List<Device> devices)
        {
            var settings = DeviceListConfig.DeviceSettings;
            foreach (var device in devices)
            {
                device.Settings = settings.Find(device.Matches) ?? device.Settings;
                settings.Remove(device.Settings);
                device.Settings.StrokerSettings = device.IsStroker
                    ? device.Settings.StrokerSettings
                    : null;
                device.Settings.VibratorSettings = device.IsVibrator
                    ? device.Settings.VibratorSettings
                    : null;
                device.Settings.ConstrictSettings = device.IsConstrictor
                    ? device.Settings.ConstrictSettings
                    : null;
            }
        }
    }
}