using UnityEngine;

namespace LoveMachine.Core
{
    public class StrokerSettings
    {
        public int MaxStrokesPerMinute { get; set; } = 150;
        public float SlowStrokeZoneMin { get; set; } = 0f;
        public float SlowStrokeZoneMax { get; set; } = 1f;
        public float FastStrokeZoneMin { get; set; } = 0.1f;
        public float FastStrokeZoneMax { get; set; } = 0.9f;
        public bool SmoothStroking { get; set; } = false;
        public float StrokeLengthRealism { get; set; } = 0f;
        public float HardSexIntensity { get; set; } = 0.2f;
        public float OrgasmDepth { get; set; } = 0.2f;
        public int OrgasmShakingFrequency { get; set; } = 10;

        public void Draw()
        {
            var defaults = new StrokerSettings();
            MaxStrokesPerMinute = GUIUtil.IntSlider(
                label: "Max Strokes (per minute)",
                tooltip: "The top speed possible on your stroker at 100% stroke length.",
                value: MaxStrokesPerMinute,
                defaultValue: defaults.MaxStrokesPerMinute,
                min: 60,
                max: 300);
            GUIUtil.SmallSpace();
            GUILayout.Label("Stroke Zones");
            { 
                float min = SlowStrokeZoneMin;
                float max = SlowStrokeZoneMax;
                GUIUtil.RangeSlider(
                    label: "Slow",
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
                    label: "Fast",
                    tooltip: "Range of stroking movement when going fast",
                    lower: ref min,
                    upper: ref max,
                    lowerDefault: defaults.FastStrokeZoneMin,
                    upperDefault: defaults.FastStrokeZoneMax);
                FastStrokeZoneMin = min;
                FastStrokeZoneMax = max;
            }
            GUIUtil.SmallSpace();
            SmoothStroking = GUIUtil.Toggle(
                label: "Smooth Stroking",
                tooltip: "Not all strokers support this.",
                value: SmoothStroking,
                defaultValue: defaults.SmoothStroking);
            GUIUtil.SmallSpace();
            StrokeLengthRealism = GUIUtil.PercentSlider(
                label: "Stroke Length Realism",
                tooltip: "0%: every stroke is full-length\n" +
                   "100%: strokes are as long as they appear in-game",
                value: StrokeLengthRealism,
                defaultValue: defaults.StrokeLengthRealism);
            GUIUtil.SmallSpace();
            HardSexIntensity = GUIUtil.PercentSlider("Hard Sex Intensity",
                "Makes hard sex animations feel hard",
                HardSexIntensity, defaults.HardSexIntensity);
            GUIUtil.SmallSpace();
            OrgasmDepth = GUIUtil.PercentSlider(
                label: "Orgasm Depth",
                tooltip: "Stroker position when orgasming (lower = deeper)",
                value: OrgasmDepth,
                defaultValue: defaults.OrgasmDepth);
            GUIUtil.SmallSpace();
            OrgasmShakingFrequency = GUIUtil.IntSlider(
                label: "Orgasm Shaking Frequency",
                tooltip: "Amount of strokes per second when orgasming",
                value: OrgasmShakingFrequency,
                defaultValue: defaults.OrgasmShakingFrequency,
                min: 3,
                max: 15);
            GUIUtil.SmallSpace();
        }
    }
}
