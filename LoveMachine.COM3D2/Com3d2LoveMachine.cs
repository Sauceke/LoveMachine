using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.COM3D2
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Com3d2LoveMachine : BaseUnityPlugin
    {
        private void Start()
        {
            CoreConfig.Logger = Logger;
            PluginInitializer.Initialize(
                plugin: this,
                girlMappingHeader: null,
                girlMappingOptions: null,
                typeof(Com3d2ButtplugVibeController),
                typeof(Com3d2ButtplugStrokerController));
            Hooks.InstallHooks();
        }
    }
}
