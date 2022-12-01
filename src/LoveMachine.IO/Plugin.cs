using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.IO
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Plugin : BaseUnityPlugin
    {
        private void Start()
        {
            this.Initialize<InsultOrderGame>(logger: Logger);
            Hooks.InstallHooks();
        }
    }
}