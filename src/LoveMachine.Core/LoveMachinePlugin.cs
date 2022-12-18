using BepInEx;

namespace LoveMachine.Core
{
    public abstract class LoveMachinePlugin<G> : BaseUnityPlugin
        where G : GameDescriptor
    {
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