using BepInEx;
using LoveMachine.Core;

namespace LoveMachine.KK
{
    [BepInProcess("Koikatu")]
    [BepInProcess("KoikatuVR")]
    [BepInProcess("Koikatsu Party")]
    [BepInProcess("Koikatsu Party VR")]
    [BepInProcess("KoikatsuSunshine")]
    [BepInProcess("KoikatsuSunshine_VR")]
    [BepInPlugin(Globals.GUID, Globals.PluginName, Globals.Version)]
    internal class Plugin : LoveMachinePlugin<KoikatsuGame>
    {
        protected override void Start()
        {
            base.Start();
            KKAnimationConfig.Initialize(this);
            var manager = Globals.ManagerObject;
            manager.AddComponent<KoikatsuAnimationController>();
            manager.AddComponent<FondleGimmick>();
        }
    }

    [BepInProcess("CharaStudio")]
    [BepInPlugin(Globals.GUID, Globals.PluginName, Globals.Version)]
    internal class StudioPlugin : LoveMachinePlugin<StudioGame>
    { }

    // To avoid conflict with the old plugin
    // 2.1.0 was the last ButtPlugin version so this should get priority from BepInEx
    [BepInPlugin("Sauceke.ButtPlugin", "ButtPlugin", "2.1.1")]
    internal class EmptyPlugin : BaseUnityPlugin
    { }
}