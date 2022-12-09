using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.HKR
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Plugin : LoveMachinePlugin<HolyKnightRiccaGame>
    {
        protected override void InstallHooks() => Hooks.InstallHooks();
    }
}