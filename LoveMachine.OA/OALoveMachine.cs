using System.Linq;
using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.OA
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    public class OALoveMachine : BaseUnityPlugin
    {
        private void Start()
        {
            var actions = new string[] { "Auto" }
                .Concat(OurApartmentButtplugController.femaleBones.Values)
                .ToArray();
            CoreConfig.Logger = Logger;
            PluginInitializer.Initialize(
                plugin: this,
                girlMappingHeader: null,
                girlMappingOptions: null,
                actionMappingHeader: "Body Part",
                actionMappingOptions: actions,
                typeof(OurApartmentButtplugStrokerController),
                typeof(OurApartmentButtplugVibrationController));
            Hooks.InstallHooks();
        }
    }
}
