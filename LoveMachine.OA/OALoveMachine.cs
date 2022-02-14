using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.OA
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    public class OALoveMachine : BaseUnityPlugin
    {
        private void Start()
        {
            CoreConfig.Logger = Logger;
            PluginInitializer.Initialize(
                plugin: this,
                girlMappingHeader: null,
                girlMappingOptions: null,
                typeof(OurApartmentButtplugStrokerController),
                typeof(OurApartmentButtplugVibrationController));
            Hooks.InstallHooks();
        }
    }
}
