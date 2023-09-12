using BepInEx;
using LoveMachine.Core;
using LoveMachine.Core.PlatformSpecific;

namespace LoveMachine.PH
{
    [BepInProcess("PlayHome32bit")]
    [BepInProcess("PlayHome64bit")]
    [BepInPlugin(Globals.GUID, Globals.PluginName, Globals.Version)]
    internal class Plugin : LoveMachinePlugin<PlayHomeGame>
    { }
}