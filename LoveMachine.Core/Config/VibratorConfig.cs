using BepInEx;
using BepInEx.Configuration;

namespace LoveMachine.Core
{
    public static class VibratorConfig
    {
        public static ConfigEntry<bool> SyncVibrationWithAnimation { get; private set; }
        public static ConfigEntry<int> VibrationUpdateFrequency { get; private set; }
        public static ConfigEntry<int> VibrationIntensityMin { get; private set; }
        public static ConfigEntry<int> VibrationIntensityMax { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            string vibrationSettingsTitle = "Vibration Settings";
            SyncVibrationWithAnimation = plugin.Config.Bind(
                section: vibrationSettingsTitle,
                key: "Vibration With Animation",
                defaultValue: true,
                "Maps vibrations to a wave pattern in sync with animations.");
            VibrationUpdateFrequency = plugin.Config.Bind(
                section: vibrationSettingsTitle,
                key: "Update Frequency (per second)",
                defaultValue: 10,
                "Average times per second we update the vibration state.");
            plugin.Config.Bind(
                section: vibrationSettingsTitle,
                key: "Vibration Intensity Range",
                defaultValue: "Ignore this",
                new ConfigDescription(
                    "Vibration intensity will oscillate between these two values.",
                    tags: new ConfigurationManagerAttributes
                    {
                        CustomDrawer = VibrationIntensityDrawer,
                        HideDefaultButton = true,
                    }));
            VibrationIntensityMin = plugin.Config.Bind(
                section: vibrationSettingsTitle,
                key: "Vibration Intensity Min",
                defaultValue: 0,
                new ConfigDescription(
                    "Lowest vibration intensity.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes
                    {
                        CustomDrawer = entry => { },
                        HideSettingName = true,
                        HideDefaultButton = true
                    }));
            VibrationIntensityMax = plugin.Config.Bind(
                section: vibrationSettingsTitle,
                key: "Vibration Intensity Max",
                defaultValue: 100,
                new ConfigDescription(
                    "Highest vibration intensity.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes
                    {
                        CustomDrawer = entry => { },
                        HideSettingName = true,
                        HideDefaultButton = true
                    }));
        }

        private static void VibrationIntensityDrawer(ConfigEntryBase obj) =>
            GUIUtil.DrawRangeSlider(VibrationIntensityMin, VibrationIntensityMax);
    }
}
