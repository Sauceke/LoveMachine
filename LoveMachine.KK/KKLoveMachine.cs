using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using LoveMachine.Core;

namespace LoveMachine.KK
{
    [BepInProcess("Koikatu")]
    [BepInProcess("KoikatuVR")]
    [BepInProcess("Koikatsu Party")]
    [BepInProcess("Koikatsu Party VR")]
    [BepInPlugin(CoreConfig.GUID, "LoveMachine", CoreConfig.Version)]
    public class KKLoveMachine : BaseUnityPlugin
    {
        public static ConfigEntry<bool> ReduceAnimationSpeeds;
        public static ConfigEntry<bool> SuppressAnimationBlending;

        private void Start()
        {
            var girls = new string[] { "First girl", "Second girl", "Off" };
            var actions = new string[] { "Auto" }
                .Concat(KoikatsuButtplugController.femaleBones.Values)
                .ToArray();
            CoreConfig.Logger = Logger;
            PluginInitializer.Initialize(
                plugin: this,
                girlMappingHeader: "Threesome Role",
                girlMappingOptions: girls,
                actionMappingHeader: "Body Part",
                actionMappingOptions: actions,
                typeof(KoikatsuButtplugAnimationController),
                typeof(KoikatsuButtplugStrokerController),
                typeof(KoikatsuButtplugVibrationController),
                typeof(KoikatsuButtplugAibuStrokerController),
                typeof(KoikatsuButtplugAibuVibrationController));
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
    }

    // To avoid conflict with the old plugin
    // 2.1.0 was the last ButtPlugin version so this should get priority from BepInEx
    [BepInPlugin("Sauceke.ButtPlugin", "ButtPlugin", "2.1.1")]
    internal class EmptyPlugin : BaseUnityPlugin { }
}
