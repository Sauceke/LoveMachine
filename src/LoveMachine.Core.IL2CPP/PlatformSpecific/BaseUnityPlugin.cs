using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace BepInEx;

public class BaseUnityPlugin : BasePlugin
{
    protected ManualLogSource Logger => Log;

    public override void Load() => Traverse.Create(this).Method("Start").GetValue();
}