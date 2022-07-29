namespace LoveMachine.Core
{
    public class VibratorSettings
    {
        public float IntensityMin { get; set; } = 0f;
        public float IntensityMax { get; set; } = 1f;

        internal void Draw()
        {
            var defaults = new VibratorSettings();
            float min = IntensityMin;
            float max = IntensityMax;
            GUIUtil.RangeSlider(
                label: "Intensity Range",
                tooltip: "Range of vibration strength",
                lower: ref min,
                upper: ref max,
                lowerDefault: defaults.IntensityMin,
                upperDefault: defaults.IntensityMax);
            IntensityMin = min;
            IntensityMax = max;
        }
    }
}
