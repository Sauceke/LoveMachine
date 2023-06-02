using BepInEx;

namespace LoveMachine.Core
{
    /// <summary>
    /// Base type for all LoveMachine plugins.
    /// </summary>
    /// <typeparam name="G">The GameDescriptor type for this plugin.</typeparam>
    public abstract class LoveMachinePlugin<G> : BaseUnityPlugin
        where G : GameDescriptor
    {
        protected virtual void Start()
        {
            Globals.Initialize(Logger);
            ButtplugConfig.Initialize(this);
            DeviceListConfig.Initialize(this, DeviceListGUI.DeviceListDrawer);
            KillSwitchConfig.Initialize(this);
            RotatorConfig.Initialize(this);
            StrokerConfig.Initialize(this);
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
            GameHooks.InstallHooks();
        }
    }
}