using System;
using System.Linq;
using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.UI.Util;
using UnityEngine;

namespace LoveMachine.Core.UI.Settings
{
    internal class VibratorSettingsUI: SettingsUI
    {
        public override void Draw(DeviceSettings deviceSettings)
        {
            var settings = deviceSettings.VibratorSettings;
            if (settings == null)
            {
                return;
            }
            var defaults = new VibratorSettings();
            {
                float min = settings.IntensityMin;
                float max = settings.IntensityMax;
                GUIUtil.PercentRangeSlider(
                    label: "Intensity Range",
                    tooltip: "Range of vibration strength",
                    lower: ref min,
                    upper: ref max,
                    lowerDefault: defaults.IntensityMin,
                    upperDefault: defaults.IntensityMax);
                settings.IntensityMin = min;
                settings.IntensityMax = max;
            }
            settings.Pattern = (VibrationPattern)GUIUtil.MultiChoice(
                label: "Vibration Pattern",
                tooltip: "Waveform of vibrations",
                choices: Enum.GetNames(typeof(VibrationPattern)),
                value: (int)settings.Pattern);
            if (settings.Pattern == VibrationPattern.Custom)
            {
                GUILayout.BeginHorizontal();
                {
                    GUIUtil.LabelWithTooltip("Custom Pattern", "Draw your own pattern.");
                    settings.CustomPattern = settings.CustomPattern
                        .Select(y => GUILayout.VerticalSlider(y, 1f, 0f))
                        .ToArray();
                }
                GUILayout.EndHorizontal();
                GUIUtil.SingleSpace();
            }
        }
    }
}