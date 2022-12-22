using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.SCS
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Plugin : LoveMachinePlugin<SecrossphereGame>
    { }
}