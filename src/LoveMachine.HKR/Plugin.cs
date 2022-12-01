using BepInEx;
using BepInEx.Unity.IL2CPP;
using LoveMachine.Core;

namespace LoveMachine.HKR
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class Plugin : BasePlugin
    {
        public override void Load()
        {
            new BaseUnityPlugin(this).Initialize<HolyKnightRiccaGame>(Log);
            Hooks.InstallHooks();
        }
    }
}