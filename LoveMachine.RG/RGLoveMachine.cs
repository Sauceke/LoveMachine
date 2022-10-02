using BepInEx;
using BepInEx.IL2CPP;
using LoveMachine.Core;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace LoveMachine.RG
{
    [BepInPlugin(CoreConfig.GUID, CoreConfig.PluginName, CoreConfig.Version)]
    internal class RGLoveMachine : BasePlugin
    {
        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<ButtplugWsClient>();
            IL2CPPChainloader.AddUnityComponent<ButtplugWsClient>();
        }
    }
}
