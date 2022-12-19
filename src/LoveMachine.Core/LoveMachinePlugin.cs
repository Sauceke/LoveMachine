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
        /// <summary>
        /// Add the following game hooks here: <br/>
        /// - call G.StartH() when an H-scene has started, <br/>
        /// - call G.EndH() when an H-scene has ended. <br/>
        /// You can use Harmony patches for both.
        /// </summary>
        protected abstract void InstallHooks();

        protected virtual void Start()
        {
            CoreConfig.Initialize(this, Logger);
            ButtplugConfig.Initialize(this);
            DeviceListConfig.Initialize(this);
            KillSwitchConfig.Initialize(this);
            RotatorConfig.Initialize(this);
            StrokerConfig.Initialize(this);
            ConstrictConfig.Initialize(this);
            var manager = CoreConfig.ManagerObject;
            manager.AddComponent<G>();
            manager.AddComponent<ButtplugWsClient>();
            manager.AddComponent<DeviceManager>();
            manager.AddComponent<AnimationAnalyzer>();
            manager.AddComponent<StrokerController>();
            manager.AddComponent<VibratorController>();
            manager.AddComponent<RotatorController>();
            manager.AddComponent<ConstrictController>();
            InstallHooks();
        }
    }
}