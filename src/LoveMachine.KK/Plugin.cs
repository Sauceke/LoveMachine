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
    [BepInPlugin(CoreConfig.GUID, "LoveMachine", CoreConfig.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private void Start()
        {
            this.Initialize<KoikatsuGame>(Logger);
            KKAnimationConfig.Initialize(this);
            var manager = CoreConfig.ManagerObject;
            manager.AddComponent<KoikatsuAnimationController>();
            manager.AddComponent<KoikatsuAibuStrokerController>();
            manager.AddComponent<KoikatsuAibuVibratorController>();
            Hooks.InstallHSceneHooks();
        }
    }

    [BepInProcess("CharaStudio")]
    [BepInPlugin(CoreConfig.GUID, "LoveMachine", CoreConfig.Version)]
    public class StudioPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            this.Initialize<StudioGame>(Logger);
            Hooks.InstallStudioHooks();
        }
    }

    // To avoid conflict with the old plugin
    // 2.1.0 was the last ButtPlugin version so this should get priority from BepInEx
    [BepInPlugin("Sauceke.ButtPlugin", "ButtPlugin", "2.1.1")]
    internal class EmptyPlugin : BaseUnityPlugin
    { }
}