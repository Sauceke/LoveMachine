using BepInEx;
using LoveMachine.Core;
using LoveMachine.Core.PlatformSpecific;

namespace LoveMachine.HS2
{
    [BepInProcess("HoneySelect2")]
    [BepInProcess("HoneySelect2VR")]
    [BepInPlugin(Globals.GUID, Globals.PluginName, Globals.Version)]
    internal class Plugin : LoveMachinePlugin<HoneySelect2Game>
    {
        protected override void Start()
        {
            base.Start();
            Globals.ManagerObject.AddComponent<SlapGimmick>();
        }
    }

    // To avoid conflict with the old plugin
    // 2.1.0 was the last ButtPlugin version so this should get priority from BepInEx
    [BepInPlugin("Sauceke.ButtPlugin", "ButtPlugin", "2.1.1")]
    internal class EmptyPlugin : BaseUnityPlugin
    { }
}