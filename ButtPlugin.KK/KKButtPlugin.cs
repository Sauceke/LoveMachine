using BepInEx;
using ButtPlugin.Core;

namespace ButtPlugin.KK
{
    [BepInPlugin(CoreConfig.GUID, "ButtPlugin", CoreConfig.Version)]
    public class KKButtPlugin : BaseUnityPlugin
    {
        void Start()
        {
            CoreConfig.Logger = Logger;
            ButtPluginInitializer<KoikatsuButtplugStrokerController, KoikatsuButtplugVibrationController>.Start(this);
            Hooks.InstallHooks();
        }
    }
}
