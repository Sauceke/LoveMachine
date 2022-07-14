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
        public static void Initialize<T>(this BaseUnityPlugin plugin, ManualLogSource logger,
            params Type[] extraControllers)
            where T : GameDescriptor
        {
            CoreConfig.Initialize(plugin, logger);
            var manager = Chainloader.ManagerObject;
            manager.AddComponent<T>().GetType();
            manager.AddComponent<ButtplugWsClient>();
            manager.AddComponent<AnimationAnalyzer>();
            manager.AddComponent<StrokerController>();
            manager.AddComponent<VibratorController>();
            manager.AddComponent<RotatorController>();
            foreach (var controller in extraControllers)
            {
                manager.AddComponent(controller);
            }
        }
    }
}
