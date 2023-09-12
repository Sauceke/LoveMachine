using BepInEx;
using LoveMachine.Core.PlatformSpecific;

namespace LoveMachine.Core
{
    // Having a plugin in the core dll will *probably* help BepInEx shim the
    // plugin correctly when its Harmony version gets outdated.
    [BepInPlugin(Globals.GUID + ".Core", Globals.PluginName + ".Core", Globals.Version)]
    internal class CorePlugin : BaseUnityPlugin
    { }
}