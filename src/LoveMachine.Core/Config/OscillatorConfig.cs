using BepInEx;
using BepInEx.Configuration;
using LoveMachine.Core.NonPortable;

namespace LoveMachine.Core.Config
{
    public class OscillatorConfig
    {
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
                    "Safety limit for maximum allowed RPM.",
                    new AcceptableValueRange<int>(60, 600),
                    new ConfigurationManagerAttributes { Order = --order }));
        }
    }
}