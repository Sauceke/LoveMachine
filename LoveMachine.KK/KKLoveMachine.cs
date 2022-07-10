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
        private void Start()
        {
            string[] girls = new string[] { "First girl", "Second girl", "Off" };
            CoreConfig.Logger = Logger;
            PluginInitializer<KoikatsuGame>.Initialize(
                plugin: this,
                girlMappingHeader: "Threesome Role",
                girlMappingOptions: girls,
                typeof(KoikatsuAnimationController),
                typeof(KoikatsuAibuStrokerController),
                typeof(KoikatsuAibuVibratorController),
                typeof(KoikatsuCalorDepthController),
                typeof(KoikatsuHotdogDepthController));
            AddExperimentalSettings();
            if (CoreConfig.EnableCalorDepthControl.Value)
            {
                Chainloader.ManagerObject.AddComponent<CalorDepthPOC>();
            }
            if (CoreConfig.EnableHotdogDepthControl.Value)
            {
                Chainloader.ManagerObject.AddComponent<HotdogDepthPOC>();
            }
            string animationSettingsTitle = "Animation Settings";
            CoreConfig.ReduceAnimationSpeeds = Config.Bind(
                section: animationSettingsTitle,
                key: "Reduce animation speeds",
                defaultValue: true,
                "Whether to slow down animations to a speed your stroker can handle");
            CoreConfig.SuppressAnimationBlending = Config.Bind(
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
                        CustomDrawer = entry =>
                            PluginInitializer<KoikatsuGame>.MakeGUILabel(experimentalNote),
                        HideSettingName = true,
                        HideDefaultButton = true
                    })
                );
            CoreConfig.EnableCalorDepthControl = Config.Bind(
                section: experimentalTitle,
                key: "Enable Lovense Calor depth control",
                defaultValue: false,
                "Use a Lovense Calor device for depth control");
            CoreConfig.EnableHotdogDepthControl = Config.Bind(
                section: experimentalTitle,
                key: "Enable Hotdog depth control",
                defaultValue: false,
                "Use a Hotdog device for depth control");
            CoreConfig.HotdogServerAddress = Config.Bind(
                section: experimentalTitle,
                key: "Hotdog server address",
                defaultValue: "ws://localhost:5365",
                "The address of the Hotdog server");
        }
    }

    // To avoid conflict with the old plugin
    // 2.1.0 was the last ButtPlugin version so this should get priority from BepInEx
    [BepInPlugin("Sauceke.ButtPlugin", "ButtPlugin", "2.1.1")]
    internal class EmptyPlugin : BaseUnityPlugin { }
}
