using LoveMachine.Core.Common;

namespace LoveMachine.Core.Settings
{
    public class DeviceSettings
    {
        public string DeviceName { get; set; }
        public int GirlIndex { get; set; } = 0;
        public Bone Bone { get; set; } = Bone.Auto;
        public int LatencyMs { get; set; } = 0;
        public int UpdatesHz { get; set; } = 10;
        public StrokerSettings StrokerSettings { get; set; } = new StrokerSettings();
        public VibratorSettings VibratorSettings { get; set; } = new VibratorSettings();
        public ConstrictSettings ConstrictSettings { get; set; } = new ConstrictSettings();
    }
}