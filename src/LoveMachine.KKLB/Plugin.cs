using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.KKLB
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Plugin : LoveMachinePlugin<KoiKoiGame>
    { }
}