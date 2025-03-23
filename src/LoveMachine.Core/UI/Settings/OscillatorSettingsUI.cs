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
            GUIUtil.RangeSlider(
                label: "RPM Range",
                tooltip: "Range of strokes per minute for this device.",
                setting: settings.RpmRange,
                defaults: defaults.RpmRange,
                min: 10,
                max: 600);
        }
    }
}