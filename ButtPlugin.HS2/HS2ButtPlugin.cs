using BepInEx;
using ButtPlugin.Core;

namespace ButtPlugin.HS2
{
    [BepInPlugin(CoreConfig.GUID, "ButtPlugin", CoreConfig.Version)]
    internal class HS2ButtPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            var girls = new string[] { "First girl", "Second girl", "Off" };
            CoreConfig.Logger = Logger;
            ButtPluginInitializer.Initialize(
                plugin: this,
                girlMappingHeader: "Threesome Role",
                girlMappingOptions: girls,
                actionMappingHeader: null,
                actionMappingOptions: null,
                typeof(HoneySelect2ButtplugStrokerController),
                typeof(HoneySelect2ButtplugVibrationController));
            Hooks.InstallHooks();
        }
    }
}
