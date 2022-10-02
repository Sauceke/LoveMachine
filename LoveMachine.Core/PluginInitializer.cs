using System;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;

namespace LoveMachine.Core
{
    // BepInEx seems to care only for direct subclasses of BaseUnityPlugin, so making a common
    // plugin base for LoveMachine is not possible.
    // This is ugly but my hands are tied.
    public static class PluginInitializer
    {
        /// <summary>
        /// LoveMachine's entry point. Call this in your plugin's Start method.
        /// </summary>
        /// <typeparam name="T">the GameDescriptor for this game</typeparam>
        /// <param name="logger">the Logger of this plugin</param>
        /// <param name="extraControllers">any additional ButtplugControllers</param>
        public static void Initialize<T>(this BaseUnityPlugin plugin, ManualLogSource logger,
            params Type[] extraControllers)
            where T : GameDescriptor
        {
            CoreConfig.Initialize(plugin, logger);
            ButtplugConfig.Initialize(plugin);
            DeviceListConfig.Initialize(plugin);
            KillSwitchConfig.Initialize(plugin);
            RotatorConfig.Initialize(plugin);
            StrokerConfig.Initialize(plugin);
            var manager = CoreConfig.ManagerObject;
            manager.AddComponent<T>();
            manager.AddComponent<IntifaceRunner>();
            manager.AddComponent<ButtplugWsClient>();
            manager.AddComponent<DeviceManager>();
            manager.AddComponent<AnimationAnalyzer>();
            manager.AddComponent<StrokerController>();
            manager.AddComponent<VibratorController>();
            manager.AddComponent<RotatorController>();
        }
    }
}
