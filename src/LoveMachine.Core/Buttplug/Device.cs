using System.Collections.Generic;
using System.Linq;
using LoveMachine.Core.Buttplug.Settings;

namespace LoveMachine.Core.Buttplug
{
    public class Device : Buttplug.Device
    {
        public override string DeviceName
        {
            get => Settings.DeviceName;
            set => Settings.DeviceName = value;
        }

        public float BatteryLevel { get; set; }
        public DeviceSettings Settings { get; set; } = new DeviceSettings();

        public Buttplug.Feature[][] AllFeatures => new[]
        {
            DeviceMessages.LinearCmd, DeviceMessages.RotateCmd, DeviceMessages.ScalarCmd
        };

        public FeatureSettings[][] AllFeatureSettings => new[]
        {
            Settings.LinearCmdSettings, Settings.RotateCmdSettings, Settings.ScalarCmdSettings
        };

        public bool HasBatteryLevel => DeviceMessages.SensorReadCmd.Any(f => f.HasBatteryLevel);

        private bool IsVibrator => DeviceMessages.ScalarCmd.Any(f => f.IsVibrator);
        private bool IsConstrictor => DeviceMessages.ScalarCmd.Any(f => f.IsConstrictor);
        private bool IsOscillator => DeviceMessages.ScalarCmd.Any(f => f.IsOscillator);
        private bool IsStroker => DeviceMessages.LinearCmd.Length > 0;

        internal bool Matches(DeviceSettings settings) => settings.DeviceName == DeviceName;

        internal void CleanUpSettings()
        {
            Settings.StrokerSettings = IsStroker ? (Settings.StrokerSettings ?? new StrokerSettings()) : null;
            Settings.VibratorSettings = IsVibrator ? (Settings.VibratorSettings ?? new VibratorSettings()) : null;
            Settings.OscillatorSettings = IsOscillator ? (Settings.OscillatorSettings ?? new OscillatorSettings()) : null;
            Settings.ConstrictSettings = IsConstrictor ? (Settings.ConstrictSettings ?? new ConstrictSettings()) : null;
            Settings.LinearCmdSettings = ResizeFeatureSettings(Settings.LinearCmdSettings, DeviceMessages.LinearCmd);
            Settings.RotateCmdSettings = ResizeFeatureSettings(Settings.RotateCmdSettings, DeviceMessages.RotateCmd);
            Settings.ScalarCmdSettings = ResizeFeatureSettings(Settings.ScalarCmdSettings, DeviceMessages.ScalarCmd);
        }

        private FeatureSettings[] ResizeFeatureSettings(FeatureSettings[] oldSettings, Buttplug.Feature[] features) =>
            features.Select((_, i) => i < oldSettings.Length ? oldSettings[i] : new FeatureSettings()).ToArray();
    }
}