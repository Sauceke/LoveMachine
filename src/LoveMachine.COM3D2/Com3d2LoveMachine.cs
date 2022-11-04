using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.COM3D2
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Com3d2LoveMachine : BaseUnityPlugin
    {
        private void Start()
        {
            this.Initialize<Com3d2Game>(Logger);
            Hooks.InstallHooks();
        }
    }
}