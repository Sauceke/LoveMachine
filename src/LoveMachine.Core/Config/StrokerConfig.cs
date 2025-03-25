using BepInEx;
using BepInEx.Configuration;
using LoveMachine.Core.NonPortable;

namespace LoveMachine.Core.Config
{
    internal static class StrokerConfig
    {
        public static IntensityConfigSettings IntensitySettings { get; private set; }
        public static ConfigEntry<int> HardSexIntensity { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            const string strokerSettingsTitle = "Stroker Settings";
            IntensitySettings = new IntensityConfigSettings(plugin, strokerSettingsTitle, ref order);
            HardSexIntensity = plugin.Config.Bind(
               section: strokerSettingsTitle,
               key: "Hard Sex Intensity",
               defaultValue: 20,
               new ConfigDescription(
                   "Makes hard sex animations feel hard",
                   new AcceptableValueRange<int>(0, 100),
                   new ConfigurationManagerAttributes { Order = --order }));
        }
    }
}