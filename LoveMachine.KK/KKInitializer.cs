using BepInEx.Bootstrap;
using LoveMachine.Core;

namespace LoveMachine.KK
{
    public partial class KKLoveMachine
    {
        public void DoGameSpecificInit()
        {
            var girls = new string[] { "First girl", "Second girl", "Off" };
            CoreConfig.Logger = Logger;
            PluginInitializer.Initialize(
                plugin: this,
                girlMappingHeader: "Threesome Role",
                girlMappingOptions: girls,
                typeof(KoikatsuButtplugAnimationController),
                typeof(KoikatsuButtplugStrokerController),
                typeof(KoikatsuButtplugVibrationController),
                typeof(KoikatsuButtplugRotatorController),
                typeof(KoikatsuButtplugAibuStrokerController),
                typeof(KoikatsuButtplugAibuVibrationController),
                typeof(KoikatsuCalorDepthController));
            AddExperimentalSettings();
            if (EnableCalorDepthControl.Value)
            {
                Chainloader.ManagerObject.AddComponent<CalorDepthPOC>();
            }
        }
    }
}
