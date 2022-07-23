using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Holf.AllForOne;
using UnityEngine;

namespace LoveMachine.Core
{
    public class CalorDepthPOC : DepthPOC
    {
        private const string ExecutableName = "BLEConsole.exe";
        private const string Script =
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

        private void Start()
        {
            string bleConsolePath = CoreConfig.PluginDirectoryPath + ExecutableName;
            if (!File.Exists(bleConsolePath))
            {
                CoreConfig.Logger.LogInfo("BLEConsole not installed, so can't run it.");
                return;
            }
            bleConsole = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = bleConsolePath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };
            bleConsole.Start();
            bleConsole.TieLifecycleToParentProcess();
            CoreConfig.Logger.LogInfo("Started BLEConsole.");
            stdin = new StreamWriter(bleConsole.StandardInput.BaseStream, Encoding.ASCII);
            stdin.WriteLine(Script);
            stdin.Flush();
            // can't do in coroutine, StandardOutput.Read blocks
            new Thread(Poll).Start();
        }

        private void OnDestroy() => bleConsole.Kill();

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
            CoreConfig.Logger.LogDebug($"Last line from BLEConsole: {data}");
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
                Depth = Mathf.Max(level - 1, 0) / 2f;
            }
        }
    }
}
