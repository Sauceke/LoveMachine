using System;
using System.Diagnostics;
using System.IO;
using Holf.AllForOne;
using UnityEngine;

namespace LoveMachine.Core
{
    internal class IntifaceRunner : MonoBehaviour
    {
        private Process intiface;

        public bool IsRunning => !intiface?.HasExited ?? false;

        private void Start()
        {
            string intifacePath = Environment
                .ExpandEnvironmentVariables(ButtplugConfig.IntifaceLocation.Value);
            int port = ButtplugConfig.WebSocketPort.Value;
            if (!File.Exists(intifacePath))
            {
                CoreConfig.Logger.LogInfo("Intiface CLI not found, so not running.");
                return;
            }
            intiface = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = intifacePath,
                    Arguments = $"--wsinsecureport {port} --stayopen",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            intiface.Start();
            intiface.TieLifecycleToParentProcess();
            CoreConfig.Logger.LogInfo("Started Intiface CLI.");
        }

        private void OnDestroy() => intiface?.Kill();

        public void Restart()
        {
            OnDestroy();
            Start();
        }
    }
}
