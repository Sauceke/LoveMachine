using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.SCS
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Plugin : BaseUnityPlugin
    {
        private void Start()
        {
            this.Initialize<SecrossphereGame>(Logger);
            Hooks.InstallHooks();
        }
    }
}