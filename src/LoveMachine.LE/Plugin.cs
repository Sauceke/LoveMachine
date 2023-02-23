using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.LE
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Plugin : LoveMachinePlugin<LastEvilGame>
    { }
}