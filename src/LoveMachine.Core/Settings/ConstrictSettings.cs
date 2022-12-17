namespace LoveMachine.Core
{
    public class ConstrictSettings
    {
        public float PressureMin { get; set; } = 0f;
        public float PressureMax { get; set; } = 1f;
        public int UpdateIntervalSecs { get; set; } = 5;

        internal void Draw()
        {
            var defaults = new ConstrictSettings();
            {
                float min = PressureMin;
                float max = PressureMax;
                GUIUtil.RangeSlider(
                    label: "Pressure Range",
                    tooltip: "Range of pressure to apply",
                    lower: ref min,
                    upper: ref max,
                    lowerDefault: defaults.PressureMin,
                    upperDefault: defaults.PressureMax);
                PressureMin = min;
                PressureMax = max;
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