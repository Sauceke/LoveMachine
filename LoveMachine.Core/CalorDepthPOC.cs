using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Holf.AllForOne;
using UnityEngine;

namespace LoveMachine.Core
{
    public class CalorDepthPOC : MonoBehaviour
    {
        private const string ExecutableName = "BLEConsole.exe";
        private const string ScriptName = "CalorBleConsoleScript.txt";

        private Process bleConsole;
        private ConcurrentQueue<float> depthReadings;

        public bool TryGetNewDepth(out float newDepth)
        {
            newDepth = float.NaN;
            while (depthReadings.TryDequeue(out float depth))
            {
                newDepth = depth;
            }
            return !float.IsNaN(newDepth);
        }

        private void Start()
        {
            depthReadings = new ConcurrentQueue<float>();
            string bleConsolePath = CoreConfig.PluginDirectoryPath + ExecutableName;
            if (!File.Exists(bleConsolePath))
            {
                CoreConfig.Logger.LogInfo("Will not run BLEConsole as it is not installed.");
                return;
            }
            string scriptPath = CoreConfig.PluginDirectoryPath + ScriptName;
            string script = File.ReadAllText(scriptPath);
            bleConsole = new Process();
            bleConsole.StartInfo = new ProcessStartInfo()
            {
                FileName = bleConsolePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            bleConsole.Start();
            bleConsole.TieLifecycleToParentProcess();
            CoreConfig.Logger.LogInfo("Started BLEConsole.");
            var stdin = new StreamWriter(bleConsole.StandardInput.BaseStream, Encoding.ASCII);
            stdin.WriteLine(script);
            stdin.Close();
            new Thread(Poll).Start();
        }

        private void Poll()
        {
            int nextChar;
            string data = "";
            while (-1 != (nextChar = bleConsole.StandardOutput.Read()))
            {
                data += (char)nextChar;
                switch ((char)nextChar)
                {
                    case '\n':
                        CoreConfig.Logger.LogInfo(data);
                        data = "";
                        break;
                    case ';':
                        ProcessOutput(data);
                        data = "";
                        break;
                }
            }
        }

        private void ProcessOutput(string data)
        {
            CoreConfig.Logger.LogDebug($"Got data from BLEConsole: {data}");
            if (data == null)
            {
                return;
            }
            var match = Regex.Match(data, @"M:S([0-3]);");
            if (match.Success)
            {
                int level = int.Parse(match.Groups[1].Value);
                float depth = level == 0 ? -1f : (level - 1) / 2f;
                CoreConfig.Logger.LogDebug($"Depth: {depth}");
                depthReadings.Enqueue(depth);
            }
        }
    }
}
