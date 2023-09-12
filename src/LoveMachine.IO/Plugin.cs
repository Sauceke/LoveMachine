using BepInEx;
using LoveMachine.Core;
using LoveMachine.Core.PlatformSpecific;

namespace LoveMachine.IO
{
    [BepInPlugin(Globals.GUID, Globals.PluginName, Globals.Version)]
    internal class Plugin : LoveMachinePlugin<InsultOrderGame>
    { }
}