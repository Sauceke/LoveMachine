namespace LoveMachine.Core
{
    public class ConstrictSettings
    {
        public float PressureMin { get; set; } = 0f;
        public float PressureMax { get; set; } = 1f;

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
        }
    }
}