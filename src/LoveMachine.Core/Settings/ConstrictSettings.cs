using LoveMachine.Core.Util;

namespace LoveMachine.Core.Settings
{
    public class ConstrictSettings
    {
        public bool Enabled { get; set; } = true;
        public float PressureMin { get; set; } = 0f;
        public float PressureMax { get; set; } = 1f;
        public float SpeedSensitivityMin { get; set; } = 1f;
        public float SpeedSensitivityMax { get; set; } = 3f;
        public int UpdateIntervalSecs { get; set; } = 5;

        internal void Draw()
        {
            var defaults = new ConstrictSettings();
            Enabled = GUIUtil.Toggle(
                label: "Enable Pressure Control",
                tooltip: "Turns pressure control on/off",
                value: Enabled,
                defaultValue: defaults.Enabled);
            {
                float min = PressureMin;
                float max = PressureMax;
                GUIUtil.PercentRangeSlider(
                    label: "Pressure Range",
                    tooltip: "Range of pressure to apply",
                    lower: ref min,
                    upper: ref max,
                    lowerDefault: defaults.PressureMin,
                    upperDefault: defaults.PressureMax);
                PressureMin = min;
                PressureMax = max;
            }
            {
                float min = SpeedSensitivityMin;
                float max = SpeedSensitivityMax;
                GUIUtil.RangeSlider(
                    label: "Speed Sensitivity Range",
                    tooltip: "Map lowest and highest pressure to these speeds (strokes per second)",
                    lower: ref min,
                    upper: ref max,
                    lowerDefault: defaults.SpeedSensitivityMin,
                    upperDefault: defaults.SpeedSensitivityMax,
                    min: 0.5f,
                    max: 5f);
                SpeedSensitivityMin = min;
                SpeedSensitivityMax = max;
            }
            UpdateIntervalSecs = GUIUtil.IntSlider(
                label: "Pressure Update Interval (seconds)",
                tooltip: "How often the pressure can be changed",
                value: UpdateIntervalSecs,
                defaultValue: defaults.UpdateIntervalSecs,
                min: 1,
                max: 10);
        }
    }
}