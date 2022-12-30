using BepInEx;
using BepInEx.Configuration;

namespace LoveMachine.Core
{
    public static class ConstrictConfig
    {
        public static ConfigEntry<ConstrictMode> Mode { get; private set; }
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
                    "Duration of a cycle in Cycle mode.",
                    new AcceptableValueRange<float>(5f, 200f),
                    new ConfigurationManagerAttributes { Order = order-- }));
            Mode = plugin.Config.Bind(
               section: constrictSettingsTitle,
               key: "Pressure Mode",
               defaultValue: ConstrictMode.Cycle,
               new ConfigDescription(
                   "Cycle: repeat building up and releasing pressure over a set duration\n" +
                   "Stroke Length: pressure is based on the in-game stroke length",
                   tags: new ConfigurationManagerAttributes { Order = order-- }));
        }

        public enum ConstrictMode
        {
            Cycle, StrokeLength
        }
    }
}