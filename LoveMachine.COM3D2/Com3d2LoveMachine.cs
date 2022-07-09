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
            PluginInitializer<Com3d2Game>.Initialize(
                plugin: this,
                girlMappingHeader: "Threesome Role",
                girlMappingOptions: new string[] { "First girl", "Second girl", "Off" });
            Hooks.InstallHooks();
        }
    }
}
