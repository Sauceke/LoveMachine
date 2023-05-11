using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.I2C
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    public class Plugin : LoveMachinePlugin<CamlannGame>
    { }
}