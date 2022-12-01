using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.SCS
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class SCSLoveMachine : BaseUnityPlugin
    {
        private void Start()
        {
            this.Initialize<SecrossphereGame>(Logger);
            Hooks.InstallHooks();
        }
    }
}