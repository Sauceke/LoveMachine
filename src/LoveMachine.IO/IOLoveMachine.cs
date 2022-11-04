using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.IO
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class IOLoveMachine : BaseUnityPlugin
    {
        private void Start()
        {
            this.Initialize<InsultOrderGame>(logger: Logger);
            Hooks.InstallHooks();
        }
    }
}