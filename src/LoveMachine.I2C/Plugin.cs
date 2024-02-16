using BepInEx;
using LoveMachine.Core;
using LoveMachine.Core.NonPortable;

namespace LoveMachine.I2C
{
    [BepInPlugin(Globals.GUID, Globals.PluginName, Globals.Version)]
    public class Plugin : LoveMachinePlugin<CamlannGame>
    { }
}