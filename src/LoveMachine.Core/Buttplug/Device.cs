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

        public bool IsVibrator => DeviceMessages.ScalarCmd?.Any(f => f.IsVibrator) ?? false;
        public bool IsConstrictor => DeviceMessages.ScalarCmd?.Any(f => f.IsConstrictor) ?? false;
        public bool IsOscillator => DeviceMessages.ScalarCmd?.Any(f => f.IsOscillator) ?? false;
        public bool IsStroker => DeviceMessages.LinearCmd != null;
        public bool IsRotator => DeviceMessages.RotateCmd != null;

        public bool HasBatteryLevel =>
            DeviceMessages.SensorReadCmd?.Any(f => f.HasBatteryLevel) ?? false;

        internal bool Matches(DeviceSettings settings) => settings.DeviceName == DeviceName;

        internal void CleanUpSettings()
        {
            Settings.StrokerSettings = IsStroker ? Settings.StrokerSettings : null;
            Settings.VibratorSettings = IsVibrator ? Settings.VibratorSettings : null;
            Settings.OscillatorSettings = IsOscillator ? Settings.OscillatorSettings : null;
            Settings.ConstrictSettings = IsConstrictor ? Settings.ConstrictSettings : null;
        }
    }
}