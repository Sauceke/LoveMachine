using BepInEx;
using BepInEx.Configuration;

namespace LoveMachine.Core
{
    public static class StrokerConfig
    {
        public static ConfigEntry<int> MaxStrokesPerMinute { get; private set; }
        public static ConfigEntry<int> LatencyMs { get; private set; }
        public static ConfigEntry<int> SlowStrokeZoneMin { get; private set; }
        public static ConfigEntry<int> SlowStrokeZoneMax { get; private set; }
        public static ConfigEntry<int> FastStrokeZoneMin { get; private set; }
        public static ConfigEntry<int> FastStrokeZoneMax { get; private set; }
        public static ConfigEntry<float> StrokeLengthRealism { get; private set; }
        public static ConfigEntry<float> OrgasmDepth { get; private set; }
        public static ConfigEntry<int> OrgasmShakingFrequency { get; private set; }
        public static ConfigEntry<int> HardSexIntensity { get; private set; }
        public static ConfigEntry<bool> SmoothStroking { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            string strokerSettingsTitle = "Stroker Settings";
            MaxStrokesPerMinute = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Maximum strokes per minute",
                defaultValue: 140,
                new ConfigDescription(
                    "The top speed possible on your stroker in your preferred Fast Stroke Zone.\n" +
                    "LoveMachine will slow down animations if necessary based on this value.",
                    new AcceptableValueRange<int>(0, 300),
                    new ConfigurationManagerAttributes { Order = order-- }));
            LatencyMs = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Latency (ms)",
                defaultValue: 0,
                new ConfigDescription(
                    "The difference in latency between your stroker and your display.\n" +
                    "Negative if your stroker has lower latency than your display.",
                    new AcceptableValueRange<int>(-500, 500),
                    new ConfigurationManagerAttributes { Order = order-- }));
            SmoothStroking = plugin.Config.Bind(
               section: strokerSettingsTitle,
               key: "Smooth Stroking",
               defaultValue: false,
               new ConfigDescription(
                   "Currently, only the Handy supports this.",
                   acceptableValues: null,
                   new ConfigurationManagerAttributes { Order = order-- }));
            SlowStrokeZoneMin = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Slow Stroke Zone Min",
                defaultValue: 0,
                new ConfigDescription(
                    "Lowest position the stroker will move to when going slow.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes
                    {
                        Order = order--,
                        CustomDrawer = entry => { },
                        HideSettingName = true,
                        HideDefaultButton = true
                    }));
            SlowStrokeZoneMax = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Slow Stroke Zone Max",
                defaultValue: 100,
                new ConfigDescription(
                    "Highest position the stroker will move to when going slow.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes
                    {
                        Order = order--,
                        CustomDrawer = entry => { },
                        HideSettingName = true,
                        HideDefaultButton = true
                    }));
            FastStrokeZoneMin = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Fast Stroke Zone Min",
                defaultValue: 10,
                new ConfigDescription(
                    "Lowest position the stroker will move to when going fast.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes
                    {
                        Order = order--,
                        CustomDrawer = entry => { },
                        HideSettingName = true,
                        HideDefaultButton = true
                    }));
            FastStrokeZoneMax = plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Fast Stroke Zone Max",
                defaultValue: 80,
                new ConfigDescription(
                    "Highest position the stroker will move to when going fast.",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes
                    {
                        Order = order--,
                        CustomDrawer = entry => { },
                        HideSettingName = true,
                        HideDefaultButton = true
                    }));
            plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Stroke Zone (Slow)",
                defaultValue: "Ignore this",
                new ConfigDescription(
                    "Range of stroking movement when going slow",
                    tags: new ConfigurationManagerAttributes
                    {
                        Order = order--,
                        CustomDrawer = SlowStrokeZoneDrawer,
                        HideDefaultButton = true,
                    }));
            plugin.Config.Bind(
                section: strokerSettingsTitle,
                key: "Stroke Zone (Fast)",
                defaultValue: "Ignore this",
                new ConfigDescription(
                    "Range of stroking movement when going fast",
                    tags: new ConfigurationManagerAttributes
                    {
                        Order = order--,
                        CustomDrawer = FastStrokeZoneDrawer,
                        HideDefaultButton = true
                    }));
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

        private static void SlowStrokeZoneDrawer(ConfigEntryBase entry) =>
            GUIUtil.DrawRangeSlider(SlowStrokeZoneMin, SlowStrokeZoneMax);

        private static void FastStrokeZoneDrawer(ConfigEntryBase entry) =>
            GUIUtil.DrawRangeSlider(FastStrokeZoneMin, FastStrokeZoneMax);
    }
}
