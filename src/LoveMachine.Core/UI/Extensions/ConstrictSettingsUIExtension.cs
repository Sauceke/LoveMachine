using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.UI.Util;

namespace LoveMachine.Core.UI.Extensions
{
    internal static class ConstrictSettingsUIExtension
    {
        public static void Draw(this ConstrictSettings settings)
        {
            var defaults = new ConstrictSettings();
            settings.Enabled = GUIUtil.Toggle(
                label: "Enable Pressure Control",
                tooltip: "Turns pressure control on/off",
                value: settings.Enabled,
                defaultValue: defaults.Enabled);
            {
                float min = settings.PressureMin;
                float max = settings.PressureMax;
                GUIUtil.PercentRangeSlider(
                    label: "Pressure Range",
                    tooltip: "Range of pressure to apply",
                    lower: ref min,
                    upper: ref max,
                    lowerDefault: defaults.PressureMin,
                    upperDefault: defaults.PressureMax);
                settings.PressureMin = min;
                settings.PressureMax = max;
            }
            {
                float min = settings.SpeedSensitivityMin;
                float max = settings.SpeedSensitivityMax;
                GUIUtil.RangeSlider(
                    label: "Speed Sensitivity Range",
                    tooltip: "Map lowest and highest pressure to these speeds (strokes per second)",
                    lower: ref min,
                    upper: ref max,
                    lowerDefault: defaults.SpeedSensitivityMin,
                    upperDefault: defaults.SpeedSensitivityMax,
                    min: 0.5f,
                    max: 5f);
                settings.SpeedSensitivityMin = min;
                settings.SpeedSensitivityMax = max;
            }
            settings.UpdateIntervalSecs = GUIUtil.IntSlider(
                label: "Pressure Update Interval (seconds)",
                tooltip: "How often the pressure can be changed",
                value: settings.UpdateIntervalSecs,
                defaultValue: defaults.UpdateIntervalSecs,
                min: 1,
                max: 10);
        }
    }
}