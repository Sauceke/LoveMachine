namespace LoveMachine.Core.Buttplug.Settings
{
    public class StrokerSettings
    {
        public int MaxStrokesPerMin { get; set; } = 150;
        public float SlowStrokeZoneMin { get; set; } = 0f;
        public float SlowStrokeZoneMax { get; set; } = 1f;
        public float FastStrokeZoneMin { get; set; } = 0.1f;
        public float FastStrokeZoneMax { get; set; } = 0.9f;
        public bool SmoothStroking { get; set; } = false;
    }
}