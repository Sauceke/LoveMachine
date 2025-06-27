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
            base.Start();
            PrototyperConfig.Initialize(this);
            Globals.ManagerObject.AddComponent<HListener>();
        }
    }
}