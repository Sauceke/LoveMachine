using BepInEx;
using BepInEx.Configuration;

namespace LoveMachine.Core
{
    public static class ConstrictConfig
    {
        public static ConfigEntry<float> CycleLengthSecs { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            const string constrictSettingsTitle = "Pressure Settings";
            CycleLengthSecs = plugin.Config.Bind(
                section: constrictSettingsTitle,
                key: "Pressure Cycle Length (seconds)",
                defaultValue: 60f,
                new ConfigDescription(
                    "Repeat building up and releasing pressure over this duration.",
                    new AcceptableValueRange<float>(5f, 200f),
                     new ConfigurationManagerAttributes { Order = order-- }));
        }
    }
}