using BepInEx;
using ButtPlugin.Core;

namespace ButtPlugin.HS2
{
    [BepInPlugin(CoreConfig.GUID, "ButtPlugin", CoreConfig.Version)]
    internal class HS2ButtPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            CoreConfig.Logger = Logger;
            ButtPluginInitializer<HoneySelect2ButtplugStrokerController, HoneySelect2ButtplugVibrationController>.Start(this);
            Hooks.InstallHooks();
        }
    }
}
