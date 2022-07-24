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
                label: "Slow",
                tooltip: "Range of stroking movement when going slow",
                lower: ref min,
                upper: ref max,
                lowerDefault: defaults.IntensityMin,
                upperDefault: defaults.IntensityMax);
            IntensityMin = min;
            IntensityMax = max;
        }
    }
}
