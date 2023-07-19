using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.UI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoveMachine.Core.UI.Extensions
{
    internal static class OscillateSettingsUIExtension
    {
        public static void Draw(this OscillateSettings settings)
        {
            var defaults = new OscillateSettings();
            settings.Enabled = GUIUtil.Toggle(
                label: "Enable Oscillation Control",
                tooltip: "Turns oscillation control on/off",
                value: settings.Enabled,
                defaultValue: defaults.Enabled);
            {
                float min = settings.SpeedMin;
                float max = settings.SpeedMax;
                GUIUtil.PercentRangeSlider(
                    label: "Oscillation Range",
                    tooltip: "Range of oscillation to apply",
                    lower: ref min,
                    upper: ref max,
                    lowerDefault: defaults.SpeedMin,
                    upperDefault: defaults.SpeedMax);
                settings.SpeedMin = min;
                settings.SpeedMax = max;
            }
            {
                float min = settings.SpeedSensitivityMin;
                float max = settings.SpeedSensitivityMax;
                GUIUtil.RangeSlider(
                    label: "Speed Sensitivity Range Oscillation",
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
                label: "Oscillation Update Interval (seconds)",
                tooltip: "How often the pressure can be changed",
                value: settings.UpdateIntervalSecs,
                defaultValue: defaults.UpdateIntervalSecs,
                min: 1,
                max: 10);
        }
    }
}
