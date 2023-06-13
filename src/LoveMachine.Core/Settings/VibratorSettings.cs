namespace LoveMachine.Core.Settings
{
    public class VibratorSettings
    {
        public float IntensityMin { get; set; } = 0f;
        public float IntensityMax { get; set; } = 1f;
        public VibrationPattern Pattern { get; set; } = VibrationPattern.Sine;
        public float[] CustomPattern { get; set; } = new float[20];
    }

    public enum VibrationPattern
    {
        Sine, Triangle, Saw, Pulse, Constant, Custom
    }
}