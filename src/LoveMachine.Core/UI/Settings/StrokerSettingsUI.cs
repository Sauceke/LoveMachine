using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.UI.Util;
using System;
using UnityEngine;

namespace LoveMachine.Core.UI.Settings
{
    internal class StrokerSettingsUI : SettingsUI
    {
        public override void Draw(DeviceSettings deviceSettings)
        {
            var settings = deviceSettings.StrokerSettings;
            if (settings == null)
            {
                return;
            }
            var defaults = new StrokerSettings();
            GUIUtil.Title("Stroker Settings");
            GUIUtil.PercentRangeSlider(
                label: "Stroke Zone",
                tooltip: "Range of the stroking movement.",
                setting: settings.StrokeZone,
                defaults: defaults.StrokeZone);
            GUIUtil.PercentRangeSlider(
                label: "Orgasm Shake Zone",
                tooltip: "Range of the shaking movement during orgasm.",
                setting: settings.OrgasmShakeZone,
                defaults: defaults.OrgasmShakeZone);
            settings.SmoothStroking = GUIUtil.Toggle(
                label: "Smooth Stroking",
                tooltip: "Makes the movement less robotic. Not all strokers support this.",
                value: settings.SmoothStroking,
                defaultValue: defaults.SmoothStroking);
            if (settings.SmoothStroking)
            {
                settings.Pattern = (StrokingPattern)GUIUtil.MultiChoice(
                    label: "Stroking Pattern",
                    tooltip: "The type of stroking motion to use",
                    choices: Enum.GetNames(typeof(StrokingPattern)),
                    value: (int)settings.Pattern);
                if (settings.Pattern == StrokingPattern.Custom)
                {
                    settings.CustomPattern = GUIUtil.PatternEditor(settings.CustomPattern);
                    GUILayout.Label("The pattern should start and end at the bottom. " +
                        "Avoid large jumps and always test before use to prevent injury.");
                }
            }
        }
    }
}