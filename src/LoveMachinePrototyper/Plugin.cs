using BepInEx;
using LoveMachine.Core;
using LoveMachine.Core.NonPortable;

namespace LoveMachinePrototyper
{
    [BepInPlugin("Sauceke.LoveMachinePrototyper", "LoveMachine Prototyping Tool", Globals.Version)]
    internal class Plugin : LoveMachinePlugin<PrototyperGame>
    {
        protected override void Start()
        {
            PrototyperConfig.Initialize(this);
            base.Start();
            Globals.ManagerObject.AddComponent<HListener>();
            if (PrototyperConfig.PluginVersion.Value != (string)PrototyperConfig.PluginVersion.DefaultValue)
            {
                Logger.LogWarning("This config file was written in LoveMachine Prototyping Tool " +
                    $"{PrototyperConfig.PluginVersion.Value}, and may not work in other versions.");
            }
            if (PrototyperConfig.GameProcessName.Value != (string)PrototyperConfig.GameProcessName.DefaultValue)
            {
                Logger.LogWarning("This config file was written for a game named " +
                    $"{PrototyperConfig.GameProcessName.Value}, and may not work in this game.");
            }
        }
    }
}