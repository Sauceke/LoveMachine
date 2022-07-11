using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.OA
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    public class OALoveMachine : BaseUnityPlugin
    {
        private void Start()
        {
            this.Initialize<OurApartmentGame>(
                logger: Logger,
                girlMappingHeader: null,
                girlMappingOptions: null);
            Hooks.InstallHooks();
        }
    }
}
