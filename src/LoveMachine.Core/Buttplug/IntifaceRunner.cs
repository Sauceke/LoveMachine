using System.Diagnostics;
using System.IO;
using Holf.AllForOne;
using LoveMachine.Core.Config;
using LoveMachine.Core.PlatformSpecific;

namespace LoveMachine.Core.Buttplug
{
    internal class IntifaceRunner : CoroutineHandler
    {
        private Process intiface;

        private void Start()
        {
            if (!ButtplugConfig.RunIntiface.Value)
            {
                Logger.LogInfo("User has disabled running Intiface Engine.");
                return;
            }
            string intifaceDirPath = Path.Combine(Globals.PluginPath, "intiface");
            string intifacePath = Path.Combine(intifaceDirPath, "intiface-engine.exe");
            if (!File.Exists(intifacePath))
            {
                Logger.LogWarning("Intiface Engine not found, so not running.");
                return;
            }
            intiface = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = intifacePath,
                    Arguments = ButtplugConfig.IntifaceArgs.Value,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            intiface.Start();
            intiface.TieLifecycleToParentProcess();
            Logger.LogInfo("Started Intiface Engine.");
        }

        private void OnDestroy() => intiface?.Kill();

        public void Restart()
        {
            OnDestroy();
            Start();
        }
    }
}