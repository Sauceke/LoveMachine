using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using LoveMachine.Core.PlatformSpecific;

namespace BepInEx;

public class BaseUnityPlugin : BasePlugin
{
    protected ManualLogSource Logger => Log;

    protected PluginInfo Info => IL2CPPChainloader.Instance.Plugins[Globals.GUID];

    public override void Load() => Traverse.Create(this).Method("Start").GetValue();
}