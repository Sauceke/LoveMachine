using BepInEx;
using BepInEx.Unity.IL2CPP;
using LoveMachine.Core;

namespace LoveMachine.RG
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class RGLoveMachine : BasePlugin
    {
        public override void Load()
        {
            new BaseUnityPlugin(this).Initialize<RoomGirlGame>(Log);
            Hooks.InstallHooks();
        }
    }
}