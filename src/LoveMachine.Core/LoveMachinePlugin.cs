using BepInEx;
using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Config;
using LoveMachine.Core.Controller;
using LoveMachine.Core.Game;
using LoveMachine.Core.PlatformSpecific;
using LoveMachine.Core.UI;

namespace LoveMachine.Core
{
    /// <summary>
    /// Base type for all LoveMachine plugins.
    /// </summary>
    /// <typeparam name="TGame">The GameAdapter type for this plugin.</typeparam>
    public abstract class LoveMachinePlugin<TGame> : BaseUnityPlugin
        where TGame : GameAdapter
    {
        protected virtual void Start()
        {
            Globals.Initialize(Logger);
            KillSwitchConfig.Initialize(this);
            ButtplugConfig.Initialize(this);
            DeviceListConfig.Initialize(this);
            StrokerConfig.Initialize(this);
            RotatorConfig.Initialize(this);
            ConstrictConfig.Initialize(this);
            var manager = Globals.ManagerObject;
            manager.AddComponent<KillSwitch>();
            manager.AddComponent<TGame>();
            manager.AddComponent<ButtplugWsClient>();
            manager.AddComponent<DeviceManager>();
            manager.AddComponent<AnimationAnalyzer>();
            manager.AddComponent<StrokerController>();
            manager.AddComponent<VibratorController>();
            manager.AddComponent<RotatorController>();
            manager.AddComponent<ConstrictController>();
            manager.AddComponent<DeviceListGUI>();
            GameHooks.InstallHooks();
        }
    }
}