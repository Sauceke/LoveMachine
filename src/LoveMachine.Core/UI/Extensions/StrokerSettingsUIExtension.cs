using LoveMachine.Core.Settings;
using LoveMachine.Core.UI.Util;

namespace LoveMachine.Core.UI.Extensions
{
    internal static class StrokerSettingsUIExtension
    {
        public static void Draw(this StrokerSettings settings)
        {
            var defaults = new StrokerSettings();
            settings.MaxStrokesPerMin = GUIUtil.IntSlider(
                label: "Max Strokes Per Minute",
                tooltip: "The top speed possible on this stroker at 100% stroke length.",
                value: settings.MaxStrokesPerMin,
                defaultValue: defaults.MaxStrokesPerMin,
                min: 60,
                max: 300);
            {
                float min = settings.SlowStrokeZoneMin;
                float max = settings.SlowStrokeZoneMax;
                GUIUtil.PercentRangeSlider(
                    label: "Stroke Zone - Slow",
                    tooltip: "Range of stroking movement when going slow",
                    lower: ref min,
                    upper: ref max,
                    lowerDefault: defaults.SlowStrokeZoneMin,
                    upperDefault: defaults.SlowStrokeZoneMax);
                settings.SlowStrokeZoneMin = min;
                settings.SlowStrokeZoneMax = max;
            }
            {
                float min = settings.FastStrokeZoneMin;
                float max = settings.FastStrokeZoneMax;
                GUIUtil.PercentRangeSlider(
                    label: "Stroke Zone - Fast",
                    tooltip: "Range of stroking movement when going fast",
                    lower: ref min,
                    upper: ref max,
                    lowerDefault: defaults.FastStrokeZoneMin,
                    upperDefault: defaults.FastStrokeZoneMax);
                settings.FastStrokeZoneMin = min;
                settings.FastStrokeZoneMax = max;
            }
            settings.SmoothStroking = GUIUtil.Toggle(
                label: "Smooth Stroking",
                tooltip: "Warning: not all strokers support this.",
                value: settings.SmoothStroking,
                defaultValue: defaults.SmoothStroking);
        }
    }
}