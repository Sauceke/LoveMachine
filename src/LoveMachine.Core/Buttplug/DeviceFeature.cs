using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoveMachine.Core.Buttplug.Settings;

namespace LoveMachine.Core.Buttplug
{
    public class DeviceFeature
    {
        public Device Device { get; }
        public Buttplug.Feature Feature { get; }
        public int FeatureIndex { get; }
        public FeatureSettings Settings { get; }

        public DeviceFeature(Device device, Buttplug.Feature feature)
        {
            Device = device;
            Feature = feature;
            var search = device.AllFeatures
                .Select(features => Array.IndexOf(features, feature))
                .ToArray();
            int featureListIndex = Enumerable.Range(0, search.Length)
                .FirstOrDefault(i => search[i] > -1);
            FeatureIndex = search[featureListIndex];
            Settings = device.Settings.UseSeparateFeatureSettings
                ? device.AllFeatureSettings[featureListIndex][FeatureIndex]
                : device.Settings.GlobalFeatureSettings;
        }
    }
}
