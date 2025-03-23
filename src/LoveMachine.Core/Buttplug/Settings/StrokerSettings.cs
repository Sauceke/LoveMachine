using LoveMachine.Core.Common;

namespace LoveMachine.Core.Buttplug.Settings
{
    public class StrokerSettings
    {
        public int MaxStrokesPerMin { get; set; } = 150;
        public RangeSetting SlowStrokeZone { get; set; } = new RangeSetting(0f, 1f);
        public RangeSetting FastStrokeZone { get; set; } = new RangeSetting(0.1f, 0.9f);
        public bool SmoothStroking { get; set; } = false;
        public StrokingPattern Pattern { get; set; } = StrokingPattern.Sine;
        public float[] CustomPattern { get; set; } = new float[20];
    }

    public enum StrokingPattern
    {
        Sine, Cups, Arches, Custom
    }
}