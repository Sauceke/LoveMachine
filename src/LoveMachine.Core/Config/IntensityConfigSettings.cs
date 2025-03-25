using BepInEx;
using BepInEx.Configuration;
using LoveMachine.Core.NonPortable;

namespace LoveMachine.Core.Config
{
    internal class IntensityConfigSettings
    {
        public ConfigEntry<IntensityMode> Mode { get; private set; }
        public ConfigEntry<float> Scale { get; private set; }
        public ConfigEntry<float> CycleLengthSecs { get; private set; }

        public IntensityConfigSettings(BaseUnityPlugin plugin, string title, ref int order)
        {
            Mode = plugin.Config.Bind(
               section: title,
               key: "Intensity Mode",
               defaultValue: IntensityMode.Cycle,
               new ConfigDescription(
                   "Cycle: intensity rises and falls over a set duration.\n" +
                   "Stroke Length: intensity is based on the in-game stroke length.\n" +
                   "Stroke Speed: intensity is based on the in-game stroke speed.",
                   tags: new ConfigurationManagerAttributes { Order = --order }));
            Scale = plugin.Config.Bind(
                section: title,
                key: "Intensity Scale",
                defaultValue: 0f,
                new ConfigDescription(
                    "0%: the Intensity Mode is not applied at all.\n" +
                    "100%: the Intensity Mode is applied at full scale.",
                    new AcceptableValueRange<float>(0f, 1f),
                    new ConfigurationManagerAttributes { Order = --order }));
            CycleLengthSecs = plugin.Config.Bind(
                section: title,
                key: "Intensity Cycle Length (seconds)",
                defaultValue: 30f,
                new ConfigDescription(
                    "Duration of a cycle in Cycle mode.",
                    new AcceptableValueRange<float>(5f, 200f),
                    new ConfigurationManagerAttributes { Order = --order }));
        }
    }

    internal enum IntensityMode
    {
        Cycle, StrokeLength, StrokeSpeed
    }
}
