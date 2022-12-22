using BepInEx;

namespace LoveMachine.Core
{
    // Having a plugin in the core dll will *probably* help BepInEx shim the
    // plugin correctly when its Harmony version gets outdated.
    [BepInPlugin(CoreConfig.GUID + ".Core", CoreConfig.PluginName + ".Core", CoreConfig.Version)]
    internal class CorePlugin : BaseUnityPlugin
    { }
}