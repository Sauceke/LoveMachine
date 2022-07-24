using UnityEngine;

namespace LoveMachine.Core
{
    public class StrokerSettings
    {
        public int MaxStrokesPerMin { get; set; } = 150;
        public float SlowStrokeZoneMin { get; set; } = 0f;
        public float SlowStrokeZoneMax { get; set; } = 1f;
        public float FastStrokeZoneMin { get; set; } = 0.1f;
        public float FastStrokeZoneMax { get; set; } = 0.9f;
        public bool SmoothStroking { get; set; } = false;

        internal void Draw()
        {
            var defaults = new StrokerSettings();
            MaxStrokesPerMin = GUIUtil.IntSlider(
                label: "Max Strokes (per minute)",
                tooltip: "The top speed possible on your stroker at 100% stroke length.",
                value: MaxStrokesPerMin,
                defaultValue: defaults.MaxStrokesPerMin,
                min: 60,
                max: 300);
            {
                float min = SlowStrokeZoneMin;
                float max = SlowStrokeZoneMax;
                GUIUtil.RangeSlider(
                    label: "Stroke Zone / Slow",
                    tooltip: "Range of stroking movement when going slow",
                    lower: ref min,
                    upper: ref max,
                    lowerDefault: defaults.SlowStrokeZoneMin,
                    upperDefault: defaults.SlowStrokeZoneMax);
                SlowStrokeZoneMin = min;
                SlowStrokeZoneMax = max;
            }
            {
                float min = FastStrokeZoneMin;
                float max = FastStrokeZoneMax;
                GUIUtil.RangeSlider(
                    label: "Stroke Zone / Fast",
                    tooltip: "Range of stroking movement when going fast",
                    lower: ref min,
                    upper: ref max,
                    lowerDefault: defaults.FastStrokeZoneMin,
                    upperDefault: defaults.FastStrokeZoneMax);
                FastStrokeZoneMin = min;
                FastStrokeZoneMax = max;
            }
            SmoothStroking = GUIUtil.Toggle(
                label: "Smooth Stroking",
                tooltip: "Warning: not all strokers support this.",
                value: SmoothStroking,
                defaultValue: defaults.SmoothStroking);
        }
    }
}
