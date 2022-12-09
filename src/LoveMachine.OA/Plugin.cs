using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.OA
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Plugin : LoveMachinePlugin<OurApartmentGame>
    {
        protected override void InstallHooks() => Hooks.InstallHooks();
    }
}