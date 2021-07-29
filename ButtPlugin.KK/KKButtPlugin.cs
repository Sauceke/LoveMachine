using BepInEx;
using ButtPlugin.Core;

namespace ButtPlugin.KK
{
    [BepInPlugin(CoreConfig.GUID, "ButtPlugin", CoreConfig.Version)]
    public class KKButtPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            CoreConfig.Logger = Logger;
            ButtPluginInitializer<KoikatsuButtplugStrokerController, KoikatsuButtplugVibrationController>.Start(this);
            Hooks.InstallHooks();
        }
    }
}
