using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.UI.Util;

namespace LoveMachine.Core.UI.Settings
{
    internal class StrokerSettingsUI: SettingsUI
    {
        public override void Draw(DeviceSettings deviceSettings)
        {
            var settings = deviceSettings.StrokerSettings;
            if (settings == null)
            {
                return;
            }
            var defaults = new StrokerSettings();
            GUIUtil.Title("Stroker Settings");
            settings.MaxStrokesPerMin = GUIUtil.IntSlider(
                label: "Max Strokes Per Minute",
                tooltip: "The top speed possible on this stroker at 100% stroke length.",
                value: settings.MaxStrokesPerMin,
                defaultValue: defaults.MaxStrokesPerMin,
                min: 60,
                max: 300);
            GUIUtil.PercentRangeSlider(
                label: "Stroke Zone - Slow",
                tooltip: "Range of stroking movement when going slow",
                setting: settings.SlowStrokeZone,
                defaults: defaults.SlowStrokeZone);
            GUIUtil.PercentRangeSlider(
                label: "Stroke Zone - Fast",
                tooltip: "Range of stroking movement when going fast",
                setting: settings.FastStrokeZone,
                defaults: defaults.FastStrokeZone);
            settings.SmoothStroking = GUIUtil.Toggle(
                label: "Smooth Stroking",
                tooltip: "Warning: not all strokers support this.",
                value: settings.SmoothStroking,
                defaultValue: defaults.SmoothStroking);
        }
    }
}