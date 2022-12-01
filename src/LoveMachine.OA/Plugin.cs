using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.OA
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private void Start()
        {
            this.Initialize<OurApartmentGame>(Logger);
            Hooks.InstallHooks();
        }
    }
}