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
    public class CalorDepthPOC : MonoBehaviour, IDepthSensor
    {
        private const string executableName = "BLEConsole.exe";
        private const string script =
            @"foreach LVS-
              if open $
                if set Custom Service: 54300001-0023-4bd4-bbd5-a6920e4c5653
                  print Found Lovense Calor device %name (MAC: %mac).
                  print Starting depth data stream.
                  subs #01
                  write #00 BM;
                  delay 36000000
                endif
              endif
            endfor";

        private Process bleConsole;
        private StreamWriter stdin;
        private ConcurrentQueue<float> depthReadings;

        public bool IsDeviceConnected { get; private set; } = false;

        public bool TryGetNewDepth(bool peek, out float newDepth)
        {
            newDepth = float.NaN;
            while (depthReadings.TryDequeue(out float depth))
            {
                newDepth = depth;
            }
            if (peek)
            {
                depthReadings.Enqueue(newDepth);
            }
            return !float.IsNaN(newDepth);
        }

        private void Start()
        {
            depthReadings = new ConcurrentQueue<float>();
            string bleConsolePath = CoreConfig.PluginDirectoryPath + executableName;
            if (!File.Exists(bleConsolePath))
            {
                CoreConfig.Logger.LogInfo("Will not run BLEConsole as it is not installed.");
                return;
            }
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
            stdin = new StreamWriter(bleConsole.StandardInput.BaseStream, Encoding.ASCII);
            stdin.WriteLine(script);
            stdin.Flush();
            // can't do in coroutine, StandardOutput.Read blocks
            new Thread(Poll).Start();
        }

        private void OnDestroy()
        {
            bleConsole.Kill();
        }

        private void Poll()
        {
            int nextChar;
            string data = "";
            while ((nextChar = bleConsole.StandardOutput.Read()) != -1)
            {
                data += (char)nextChar;
                switch ((char)nextChar)
                {
                    case '\n':
                        CoreConfig.Logger.LogInfo($"Got data from BLEConsole: {data}");
                        data = "";
                        break;
                    case ';':
                        CoreConfig.Logger.LogDebug($"Got data from BLEConsole: {data}");
                        ProcessOutput(data);
                        data = "";
                        break;
                }
            }
        }

        private void ProcessOutput(string data)
        {
            if (data == null)
            {
                return;
            }
            var match = Regex.Match(data, @"M:S([0-3]);");
            if (match.Success)
            {
                IsDeviceConnected = true;
                int level = int.Parse(match.Groups[1].Value);
                float depth = level == 0 ? -1f : (level - 1) / 2f;
                CoreConfig.Logger.LogDebug($"Depth: {depth}");
                depthReadings.Enqueue(depth);
            }
        }
    }
}
