using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using LoveMachine.Core;

namespace LoveMachine.KK
{
    [BepInProcess("Koikatu")]
    [BepInProcess("KoikatuVR")]
    [BepInProcess("Koikatsu Party")]
    [BepInProcess("Koikatsu Party VR")]
    [BepInProcess("KoikatsuSunshine")]
    [BepInProcess("KoikatsuSunshine_VR")]
    [BepInPlugin(CoreConfig.GUID, "LoveMachine", CoreConfig.Version)]
    public class KKLoveMachine : BaseUnityPlugin
    {
        public static ConfigEntry<bool> ReduceAnimationSpeeds;
        public static ConfigEntry<bool> SuppressAnimationBlending;
        public static ConfigEntry<bool> EnableCalorDepthControl;
        public static ConfigEntry<bool> EnableHotdogDepthControl;

        private void Start()
        {
            var girls = new string[] { "First girl", "Second girl", "Off" };
            CoreConfig.Logger = Logger;
            PluginInitializer.Initialize(
                plugin: this,
                girlMappingHeader: "Threesome Role",
                girlMappingOptions: girls,
                typeof(KoikatsuButtplugAnimationController),
                typeof(KoikatsuButtplugStrokerController),
                typeof(KoikatsuButtplugVibrationController),
                typeof(KoikatsuButtplugRotatorController),
                typeof(KoikatsuButtplugAibuStrokerController),
                typeof(KoikatsuButtplugAibuVibrationController),
                typeof(KoikatsuCalorDepthController),
                typeof(KoikatsuHotdogDepthController));
            AddExperimentalSettings();
            if (EnableCalorDepthControl.Value)
            {
                Chainloader.ManagerObject.AddComponent<CalorDepthPOC>();
            }
            if (EnableHotdogDepthControl.Value)
            {
                Chainloader.ManagerObject.AddComponent<HotdogDepthPOC>();
            }
            string animationSettingsTitle = "Animation Settings";
            ReduceAnimationSpeeds = Config.Bind(
                section: animationSettingsTitle,
                key: "Reduce animation speeds",
                defaultValue: true,
                "Whether to slow down animations to a speed your stroker can handle");
            SuppressAnimationBlending = Config.Bind(
                section: animationSettingsTitle,
                key: "Simplify animations",
                defaultValue: true,
                "Some animations are too complex and cannot be tracked precisely.\n" +
                "This setting will make such animations simpler for better immersion.");
            Hooks.InstallHooks();
        }

        private void AddExperimentalSettings()
        {
            string experimentalTitle = "Experimental Features";
            string experimentalNote = "These settings will only take effect after " +
                "restarting the game. It is recommended that you close Intiface when " +
                "using any of these features.";
            Config.Bind(
                section: experimentalTitle,
                key: "a",
                defaultValue: 0,
                new ConfigDescription(
                    "",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes
                    {
                        Order = 100,
                        CustomDrawer = entry => PluginInitializer.MakeGUILabel(experimentalNote),
                        HideSettingName = true,
                        HideDefaultButton = true
                    })
                );
            EnableCalorDepthControl = Config.Bind(
                section: experimentalTitle,
                key: "Enable Lovense Calor depth control",
                defaultValue: false,
                "Use a Lovense Calor device for depth control");
            EnableHotdogDepthControl = Config.Bind(
                section: experimentalTitle,
                key: "Enable Hotdog depth control",
                defaultValue: false,
                "Use a Hotdog device for depth control");
        }
    }

    // To avoid conflict with the old plugin
    // 2.1.0 was the last ButtPlugin version so this should get priority from BepInEx
    [BepInPlugin("Sauceke.ButtPlugin", "ButtPlugin", "2.1.1")]
    internal class EmptyPlugin : BaseUnityPlugin { }
}
