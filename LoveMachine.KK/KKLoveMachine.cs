using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.KK
{
    [BepInPlugin(CoreConfig.GUID, "LoveMachine", CoreConfig.Version)]
    public class KKLoveMachine : BaseUnityPlugin
    {
        private void Start()
        {
            var girls = new string[] { "First girl", "Second girl", "Off" };
            var actions = new string[]
            {
                "Sex", "Left Breast", "Right Breast", "Touch Groin", "Touch Anal", "Left Butt", "Right Butt"
            };
            CoreConfig.Logger = Logger;
            PluginInitializer.Initialize(
                plugin: this,
                girlMappingHeader: "Threesome Role",
                girlMappingOptions: girls,
                actionMappingHeader: "Action Mapping",
                actionMappingOptions: actions,
                typeof(KoikatsuButtplugStrokerController),
                typeof(KoikatsuButtplugVibrationController));
            Hooks.InstallHooks();
        }
    }

    // To avoid conflict with the old plugin
    // 2.1.0 was the last ButtPlugin version so this should get priority from BepInEx
    [BepInPlugin("Sauceke.ButtPlugin", "ButtPlugin", "2.1.1")]
    internal class EmptyPlugin : BaseUnityPlugin { }
}
