using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.COM3D2
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Plugin : BaseUnityPlugin
    {
        private void Start()
        {
            this.Initialize<Com3d2Game>(Logger);
            Hooks.InstallHooks();
        }
    }
}