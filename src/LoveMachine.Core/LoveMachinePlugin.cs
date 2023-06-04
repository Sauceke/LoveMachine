using BepInEx;

namespace LoveMachine.Core
{
    /// <summary>
    /// Base type for all LoveMachine plugins.
    /// </summary>
    /// <typeparam name="G">The GameAdapter type for this plugin.</typeparam>
    public abstract class LoveMachinePlugin<G> : BaseUnityPlugin
        where G : GameAdapter
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
            manager.AddComponent<G>();
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