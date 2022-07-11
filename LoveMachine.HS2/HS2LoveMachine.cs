using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.HS2
{
    [BepInProcess("HoneySelect2")]
    [BepInProcess("HoneySelect2VR")]
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class HS2LoveMachine : BaseUnityPlugin
    {
        private void Start()
        {
            string[] girls = new string[] { "First girl", "Second girl", "Off" };
            this.Initialize<HoneySelect2Game>(
                logger: Logger,
                girlMappingHeader: "Threesome Role",
                girlMappingOptions: girls);
            Hooks.InstallHooks();
        }
    }

    // To avoid conflict with the old plugin
    // 2.1.0 was the last ButtPlugin version so this should get priority from BepInEx
    [BepInPlugin("Sauceke.ButtPlugin", "ButtPlugin", "2.1.1")]
    internal class EmptyPlugin : BaseUnityPlugin { }
}
