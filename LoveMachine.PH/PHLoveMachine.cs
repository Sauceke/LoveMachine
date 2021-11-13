using System.Linq;
using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.PH
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class PHLoveMachine : BaseUnityPlugin
    {
        private void Start()
        {
            var girls = new string[] { "First girl", "Second girl", "Off" };
            var actions = new string[] { "Auto" }
                .Concat(PlayHomeButtplugController.femaleBones.Values)
                .ToArray();
            CoreConfig.Logger = Logger;
            PluginInitializer.Initialize(
                plugin: this,
                girlMappingHeader: "Threesome Role",
                girlMappingOptions: girls,
                actionMappingHeader: "Body Part",
                actionMappingOptions: actions,
                typeof(PlayHomeButtplugVibrationController),
                typeof(PlayHomeButtplugStrokerController));
            Hooks.InstallHooks();
        }
    }
}
