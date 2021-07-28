using BepInEx;
using ButtPlugin.Core;

namespace ButtPlugin.HS2
{
    [BepInPlugin(CoreConfig.GUID, "ButtPlugin", CoreConfig.Version)]
    class HS2ButtPlugin : BaseUnityPlugin
    {
        void Start()
        {
            CoreConfig.Logger = Logger;
            ButtPluginInitializer<HoneySelect2ButtplugStrokerController, HoneySelect2ButtplugVibrationController>.Start(this);
            Hooks.InstallHooks();
        }
    }
}
