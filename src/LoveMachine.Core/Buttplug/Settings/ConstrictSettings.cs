using LoveMachine.Core.Common;

namespace LoveMachine.Core.Buttplug.Settings
{
    public class ConstrictSettings
    {
        public RangeSetting PressureRange { get; set; } = new RangeSetting(0f, 1f);
        public int UpdateIntervalSecs { get; set; } = 5;
    }
}