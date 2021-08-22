using BepInEx;
using ButtPlugin.Core;

namespace ButtPlugin.KK
{
    [BepInPlugin(CoreConfig.GUID, "ButtPlugin", CoreConfig.Version)]
    public class KKButtPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            var girls = new string[] { "First girl", "Second girl", "Off" };
            var actions = new string[]
            {
                "Sex", "Left Breast", "Right Breast", "Touch Groin", "Touch Anal", "Left Butt", "Right Butt"
            };
            CoreConfig.Logger = Logger;
            ButtPluginInitializer.Initialize(
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
}
