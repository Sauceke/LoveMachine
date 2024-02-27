using LoveMachine.Core.Common;

namespace LoveMachine.Core.Buttplug.Settings
{
    public class ConstrictSettings
    {
        public bool Enabled { get; set; } = true;
        public RangeSetting PressureRange { get; set; } = new RangeSetting(0f, 1f);
        public RangeSetting SpeedSensitivityRange { get; set; } = new RangeSetting(1f, 3f);
        public int UpdateIntervalSecs { get; set; } = 5;
    }
}