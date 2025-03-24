using LoveMachine.Core.Common;

namespace LoveMachine.Core.Buttplug.Settings
{
    public class StrokerSettings
    {
        public RangeSetting StrokeZone { get; set; } = new RangeSetting(0f, 1f);
        public RangeSetting OrgasmShakeZone { get; set; } = new RangeSetting(0.2f, 0.3f);
        public bool SmoothStroking { get; set; } = false;
        public StrokingPattern Pattern { get; set; } = StrokingPattern.Sine;
        public float[] CustomPattern { get; set; } = new float[20];
    }

    public enum StrokingPattern
    {
        Sine, Cups, Arches, Custom
    }
}