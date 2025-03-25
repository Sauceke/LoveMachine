using BepInEx;
using LoveMachine.Core.NonPortable;

namespace LoveMachine.Core.Config
{
    internal static class VibratorConfig
    {
        public static IntensityConfigSettings IntensitySettings { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            const string constrictSettingsTitle = "Vibrator Settings";
            IntensitySettings = new IntensityConfigSettings(plugin, constrictSettingsTitle, ref order);
        }
    }
}
