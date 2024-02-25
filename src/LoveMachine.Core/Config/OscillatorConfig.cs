using BepInEx;
using BepInEx.Configuration;
using LoveMachine.Core.NonPortable;

namespace LoveMachine.Core.Config
{
    public class OscillatorConfig
    {
        public static ConfigEntry<OscillationMode> Mode { get; private set; }
        public static ConfigEntry<int> RpmLimit { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            const string oscillatorSettingsTitle = "Oscillation Settings";
            RpmLimit = plugin.Config.Bind(
                section: oscillatorSettingsTitle,
                key: "RPM limit",
                defaultValue: 300,
                new ConfigDescription(
                    "Maximum allowed RPM on this device.",
                    new AcceptableValueRange<int>(60, 600),
                    new ConfigurationManagerAttributes { Order = --order }));
            Mode = plugin.Config.Bind(
                section: oscillatorSettingsTitle,
                key: "Oscillation Mode",
                defaultValue: OscillationMode.Speed,
                new ConfigDescription(
                    "Speed: try to match the stroking speed\n" +
                    "Depth: increase and decrease speed with each stroke",
                    tags: new ConfigurationManagerAttributes { Order = --order }));
        }

        public enum OscillationMode
        {
            Speed, Depth
        }
    }
}