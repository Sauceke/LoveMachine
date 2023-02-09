using System;
using System.Linq;
using UnityEngine;

namespace LoveMachine.Core
{
    public class VibratorSettings
    {
        public float IntensityMin { get; set; } = 0f;
        public float IntensityMax { get; set; } = 1f;
        public VibrationPattern Pattern { get; set; } = VibrationPattern.Sine;
        public float[] CustomPattern { get; set; } = new float[20];

        internal void Draw()
        {
            var defaults = new VibratorSettings();
            {
                float min = IntensityMin;
                float max = IntensityMax;
                GUIUtil.PercentRangeSlider(
                    label: "Intensity Range",
                    tooltip: "Range of vibration strength",
                    lower: ref min,
                    upper: ref max,
                    lowerDefault: defaults.IntensityMin,
                    upperDefault: defaults.IntensityMax);
                IntensityMin = min;
                IntensityMax = max;
            }
            Pattern = (VibrationPattern)GUIUtil.MultiChoice(
                label: "Vibration Pattern",
                tooltip: "Waveform of vibrations",
                choices: Enum.GetNames(typeof(VibrationPattern)),
                value: (int)Pattern);
            if (Pattern == VibrationPattern.Custom)
            {
                GUILayout.BeginHorizontal();
                {
                    GUIUtil.LabelWithTooltip("Custom Pattern", "Draw your own pattern.");
                    CustomPattern = CustomPattern
                        .Select(y => GUILayout.VerticalSlider(y, 1f, 0f))
                        .ToArray();
                }
                GUILayout.EndHorizontal();
                GUIUtil.SingleSpace();
            }
        }
    }

    public enum VibrationPattern
    {
        Sine, Triangle, Saw, Pulse, Constant, Custom
    }
}