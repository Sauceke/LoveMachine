using BepInEx;
using BepInEx.IL2CPP;
using Il2CppSystem;
using LoveMachine.Core;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
