using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.IO
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Plugin : LoveMachinePlugin<InsultOrderGame>
    {
        protected override void InstallHooks() => Hooks.InstallHooks();
    }
}