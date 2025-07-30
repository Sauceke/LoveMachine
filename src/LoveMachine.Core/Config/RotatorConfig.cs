using BepInEx;
using BepInEx.Configuration;
using LoveMachine.Core.NonPortable;

namespace LoveMachine.Core.Config
{
    internal static class RotatorConfig
    {
        public static IntensityConfigSettings IntensitySettings { get; private set; }
        public static ConfigEntry<float> RotationDirectionChangeChance { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            const string rotationSettingsTitle = "Rotator Settings";
            IntensitySettings = new IntensityConfigSettings(plugin, rotationSettingsTitle, ref order);
            RotationDirectionChangeChance = plugin.Config.Bind(
                section: rotationSettingsTitle,
                key: "Rotation Direction Change Chance",
                defaultValue: 0.3f,
                new ConfigDescription(
                    "The direction of rotation changes with the probability of this setting",
                    new AcceptableValueRange<float>(0f, 1f),
                    new ConfigurationManagerAttributes { Order = --order }));
        }
    }
}