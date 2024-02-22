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
            settings.MaxRpm = GUIUtil.IntSlider(
                label: "Maximum RPM",
                tooltip: "Rotations per minute when this device oscillates at full speed.",
                value: settings.MaxRpm,
                defaultValue: defaults.MaxRpm,
                min: 60,
                max: 600);
            settings.SpeedMixing = GUIUtil.Toggle(
                label: "Speed Mixing",
                tooltip: "Match speeds more accurately by modulating between two speeds.",
                value: settings.SpeedMixing,
                defaultValue: defaults.SpeedMixing);
        }
    }
}