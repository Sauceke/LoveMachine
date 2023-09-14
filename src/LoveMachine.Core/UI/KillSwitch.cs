using LoveMachine.Core.Buttplug;
using LoveMachine.Core.PlatformSpecific;

namespace LoveMachine.Core.UI
{
    public class KillSwitch: CoroutineHandler
    {
        private ButtplugWsClient client;
        
        public void Start()
        {
            client = GetComponent<ButtplugWsClient>();
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