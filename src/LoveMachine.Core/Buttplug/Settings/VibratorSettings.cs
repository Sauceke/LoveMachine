using LoveMachine.Core.Common;

namespace LoveMachine.Core.Buttplug.Settings
{
    public class VibratorSettings
    {
        public RangeSetting IntensityRange { get; set; } = new RangeSetting(0f, 1f);
        public VibrationPattern Pattern { get; set; } = VibrationPattern.Sine;
        public float[] CustomPattern { get; set; } = new float[20];
    }

    public enum VibrationPattern
    {
        Sine, Triangle, Saw, Pulse, Constant, Custom
    }
}