using LoveMachine.Core.Common;

namespace LoveMachine.Core.Buttplug.Settings
{
    public class OscillatorSettings
    {
        public RangeSetting RpmRange { get; set; } = new RangeSetting(15f, 300f);
    }
}