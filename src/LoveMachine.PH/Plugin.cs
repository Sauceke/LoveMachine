using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.PH
{
    [BepInProcess("PlayHome32bit")]
    [BepInProcess("PlayHome64bit")]
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Plugin : LoveMachinePlugin<PlayHomeGame>
    {
        protected override void InstallHooks() => Hooks.InstallHooks();
    }
}