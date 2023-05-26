using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.OT
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Plugin : LoveMachinePlugin<OedoTriggerGame>
    { }
}