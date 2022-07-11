using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.PH
{
    [BepInProcess("PlayHome32bit")]
    [BepInProcess("PlayHome64bit")]
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class PHLoveMachine : BaseUnityPlugin
    {
        private void Start()
        {
            string[] girls = new string[] { "First girl", "Second girl", "Off" };
            this.Initialize<PlayHomeGame>(
                logger: Logger,
                girlMappingHeader: "Threesome Role",
                girlMappingOptions: girls);
            Hooks.InstallHooks();
        }
    }
}
