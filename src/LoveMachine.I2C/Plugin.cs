using BepInEx;
using LoveMachine.Core;
using LoveMachine.Core.PlatformSpecific;

namespace LoveMachine.I2C
{
    [BepInPlugin(Globals.GUID, Globals.PluginName, Globals.Version)]
    public class Plugin : LoveMachinePlugin<CamlannGame>
    { }
}