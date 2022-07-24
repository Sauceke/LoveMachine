using BepInEx;
using BepInEx.Configuration;

namespace LoveMachine.Core
{
    public static class VibratorConfig
    {
        public static ConfigEntry<bool> SyncVibrationWithAnimation { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            string vibrationSettingsTitle = "Vibration Settings";
            SyncVibrationWithAnimation = plugin.Config.Bind(
                section: vibrationSettingsTitle,
                key: "Vibration With Animation",
                defaultValue: true,
                "Maps vibrations to a wave pattern in sync with animations.");
        }
    }
}
