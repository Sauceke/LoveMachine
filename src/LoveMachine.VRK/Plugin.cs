using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.VRK
{
    [BepInProcess("VR_Kanojo")]
    [BepInPlugin(Globals.GUID, Globals.PluginName, Globals.Version)]
    internal class Plugin : LoveMachinePlugin<VRKanojoGame>
    { }
}