using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Game;
using LoveMachine.Core.NonPortable;

namespace LoveMachine.Core.UI
{
    public class KillSwitch: CoroutineHandler
    {
        private ButtplugWsClient client;
        
        public void Start()
        {
            client = GetComponent<ButtplugWsClient>();
            var game = GetComponent<GameAdapter>();
            game.OnHStarted += (s, a) => enabled = true;
            game.OnHEnded += (s, a) => enabled = false;
            enabled = false;
        }
        
        public void Update()
        {
            if (KillSwitchConfig.KillSwitch.Value.IsDown())
            {
                client.StopAllDevices();
                client.IsConsensual = false;
                Logger.LogMessage("LoveMachine: Emergency stop pressed.");
            }
            else if (KillSwitchConfig.ResumeSwitch.Value.IsPressed())
            {
                client.IsConsensual = true;
            }
        }
    }
}