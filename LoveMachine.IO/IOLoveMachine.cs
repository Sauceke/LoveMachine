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
            PluginInitializer.Initialize(
                plugin: this,
                girlMappingHeader: null,
                girlMappingOptions: null,
                typeof(InsultOrderButtplugVibrationController),
                typeof(InsultOrderButtplugStrokerController),
                typeof(InsultOrderButtplugRotatorController));
            Hooks.InstallHooks();
        }
    }
}
