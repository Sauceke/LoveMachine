using BepInEx;
using BepInEx.Configuration;

namespace LoveMachine.Core
{
    public static class RotatorConfig
    {
        public static ConfigEntry<float> RotationSpeedRatio { get; private set; }
        public static ConfigEntry<float> RotationDirectionChangeChance { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            string rotationSettingsTitle = "Rotation Settings";
            RotationSpeedRatio = plugin.Config.Bind(
                section: rotationSettingsTitle,
                key: "Rotation Speed Ratio",
                defaultValue: 0.5f,
                new ConfigDescription(
                    "0%: No rotation\n" +
                    "100%: Full speed rotation",
                    new AcceptableValueRange<float>(0, 1)));
            RotationDirectionChangeChance = plugin.Config.Bind(
                section: rotationSettingsTitle,
                key: "Rotation Direction Change Chance",
                defaultValue: 0.3f,
                new ConfigDescription(
                    "The direction of rotation changes with the probability of this setting",
                    new AcceptableValueRange<float>(0, 1),
                    new ConfigurationManagerAttributes { Order = order-- }));
        }
    }
}
