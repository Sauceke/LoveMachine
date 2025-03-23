using System;
using System.Linq;
using LoveMachine.Core.Buttplug;
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
            GUIUtil.Title("Vibration Settings");
            GUIUtil.PercentRangeSlider(
                label: "Intensity Range",
                tooltip: "Range of vibration strength",
                setting: settings.IntensityRange,
                defaults: defaults.IntensityRange);
            settings.Pattern = (VibrationPattern)GUIUtil.MultiChoice(
                label: "Vibration Pattern",
                tooltip: "The type of vibration sequence to use",
                choices: Enum.GetNames(typeof(VibrationPattern)),
                value: (int)settings.Pattern);
            if (settings.Pattern == VibrationPattern.Custom)
            {
                settings.CustomPattern = GUIUtil.PatternEditor(settings.CustomPattern);
            }
        }
    }
}