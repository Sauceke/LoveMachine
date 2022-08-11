using BepInEx;
using BepInEx.Bootstrap;
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
    public class KKLoveMachine : BaseUnityPlugin
    {
        private void Start()
        {
            this.Initialize<KoikatsuGame>(Logger,
                typeof(KoikatsuAnimationController),
                typeof(KoikatsuAibuStrokerController),
                typeof(KoikatsuAibuVibratorController),
                typeof(KoikatsuCalorDepthController),
                typeof(KoikatsuHotdogDepthController));
            KKAnimationConfig.Initialize(this);
            ExperimentalConfig.Initialize(this);
            if (ExperimentalConfig.EnableCalorDepthControl.Value)
            {
                Chainloader.ManagerObject.AddComponent<CalorDepthPOC>();
            }
            if (ExperimentalConfig.EnableHotdogDepthControl.Value)
            {
                Chainloader.ManagerObject.AddComponent<HotdogDepthPOC>();
            }
            Hooks.InstallHooks();
        }
    }

    // To avoid conflict with the old plugin
    // 2.1.0 was the last ButtPlugin version so this should get priority from BepInEx
    [BepInPlugin("Sauceke.ButtPlugin", "ButtPlugin", "2.1.1")]
    internal class EmptyPlugin : BaseUnityPlugin { }
}
