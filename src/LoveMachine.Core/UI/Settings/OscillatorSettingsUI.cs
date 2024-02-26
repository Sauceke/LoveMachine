using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.UI.Util;

namespace LoveMachine.Core.UI.Settings
{
    internal class OscillatorSettingsUI : SettingsUI
    {
        public override void Draw(DeviceSettings deviceSettings)
        {
            var settings = deviceSettings.OscillatorSettings;
            if (settings == null)
            {
                return;
            }
            var defaults = new OscillatorSettings();
            {
                float min = settings.MinRpm;
                float max = settings.MaxRpm;
                GUIUtil.RangeSlider(
                    label: "RPM Range",
                    tooltip: "Range of strokes per minute for this device.",
                    lower: ref min,
                    upper: ref max,
                    lowerDefault: defaults.MinRpm,
                    upperDefault: defaults.MaxRpm,
                    min: 10,
                    max: 600);
                settings.MinRpm = (int)min;
                settings.MaxRpm = (int)max;
            }
            settings.SpeedMixing = GUIUtil.Toggle(
                label: "Speed Mixing",
                tooltip: "Match speeds more accurately by modulating between two speeds.",
                value: settings.SpeedMixing,
                defaultValue: defaults.SpeedMixing);
        }
    }
}