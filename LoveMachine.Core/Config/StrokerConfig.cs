using BepInEx;
using BepInEx.Configuration;

namespace LoveMachine.Core
{
    public static class StrokerConfig
    {
        public static ConfigEntry<float> StrokeLengthRealism { get; private set; }
        public static ConfigEntry<int> HardSexIntensity { get; private set; }
        public static ConfigEntry<float> OrgasmDepth { get; private set; }
        public static ConfigEntry<int> OrgasmShakingFrequency { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            const string strokerSettingsTitle = "Stroker Settings";
            StrokeLengthRealism = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Stroke Length Realism",
                defaultValue: 0f,
                 new ConfigDescription(
                   "0%: every stroke is full-length\n" +
                   "100%: strokes are as long as they appear in-game",
                   new AcceptableValueRange<float>(0f, 1f),
                   new ConfigurationManagerAttributes { Order = order-- }));
            HardSexIntensity = plugin.Config.Bind(
               section: strokerSettingsTitle,
               key: "Hard Sex Intensity",
               defaultValue: 20,
               new ConfigDescription(
                   "Makes hard sex animations feel hard",
                   new AcceptableValueRange<int>(0, 100),
                   new ConfigurationManagerAttributes { Order = order-- }));
            OrgasmDepth = plugin.Config.Bind(
               section: strokerSettingsTitle,
               key: "Orgasm Depth",
               defaultValue: 0.2f,
               new ConfigDescription(
                   "Stroker position when orgasming (lower = deeper)",
                   new AcceptableValueRange<float>(0f, 1f),
                   new ConfigurationManagerAttributes { Order = order-- }));
            OrgasmShakingFrequency = plugin.Config.Bind(
               section: strokerSettingsTitle,
               key: "Orgasm Shaking Frequency",
               defaultValue: 10,
               new ConfigDescription(
                   "Amount of strokes per second when orgasming",
                   new AcceptableValueRange<int>(3, 15),
                   new ConfigurationManagerAttributes { Order = order-- }));
        }
    }
}
