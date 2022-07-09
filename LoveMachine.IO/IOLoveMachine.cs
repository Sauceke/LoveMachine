using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.IO
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class IOLoveMachine : BaseUnityPlugin
    {
        private void Start()
        {
            CoreConfig.Logger = Logger;
            PluginInitializer<InsultOrderGame>.Initialize(
                plugin: this,
                girlMappingHeader: null,
                girlMappingOptions: null);
            Hooks.InstallHooks();
        }
    }
}
