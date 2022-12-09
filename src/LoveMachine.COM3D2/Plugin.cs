using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.COM3D2
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Plugin : LoveMachinePlugin<Com3d2Game>
    {
        protected override void InstallHooks() => Hooks.InstallHooks();
    }
}