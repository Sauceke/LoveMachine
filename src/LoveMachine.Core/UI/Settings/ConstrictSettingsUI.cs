using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.UI.Util;

namespace LoveMachine.Core.UI.Settings
{
    internal class ConstrictSettingsUI: SettingsUI
    {
        public override void Draw(DeviceSettings deviceSettings)
        {
            var settings = deviceSettings.ConstrictSettings;
            if (settings == null)
            {
                return;
            }
            var defaults = new ConstrictSettings();
            GUIUtil.Title("Pressure Settings");
            settings.Enabled = GUIUtil.Toggle(
                label: "Enable Pressure Control",
                tooltip: "Turns pressure control on/off",
                value: settings.Enabled,
                defaultValue: defaults.Enabled);
            GUIUtil.PercentRangeSlider(
                label: "Pressure Range",
                tooltip: "Range of pressure to apply",
                setting: settings.PressureRange,
                defaults: defaults.PressureRange);
            GUIUtil.RangeSlider(
                label: "Speed Sensitivity Range",
                tooltip: "Map lowest and highest pressure to these speeds (strokes per second)",
                setting: settings.SpeedSensitivityRange,
                defaults: defaults.SpeedSensitivityRange,
                min: 0.5f,
                max: 5f);
            settings.UpdateIntervalSecs = GUIUtil.IntSlider(
                label: "Pressure Update Interval (seconds)",
                tooltip: "How often the pressure can be changed",
                value: settings.UpdateIntervalSecs,
                defaultValue: defaults.UpdateIntervalSecs,
                min: 1,
                max: 10);
        }
    }
}