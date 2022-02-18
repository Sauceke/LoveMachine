using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LoveMachine.Core
{
    public class HotdogDepthPOC : CoroutineHandler, IDepthSensor
    {
        private const string ProtocolId = "HQNDmlUP";
        private const string DepthKey = "lvl";
        private const char Separator = ' ';

        private readonly List<SerialPort> ports = new List<SerialPort>();
        private SerialPort hotdogPort;
        private float? depth = null;

        public bool IsDeviceConnected { get; private set; } = false;

        public bool TryGetNewDepth(bool peek, out float newDepth)
        {
            if (!depth.HasValue)
            {
                newDepth = 0f;
                return false;
            }
            newDepth = depth < 0.1f ? -1 : Mathf.InverseLerp(0.1f, 1f, depth.Value);
            depth = peek ? depth : null;
            return true;
        }

        private void Start()
        {
            foreach (string name in SerialPort.GetPortNames())
            {
                // GetPortNames has a bug in win10, port names sometimes have garbage characters
                // at the end
                string fixedName = Regex.Replace(name, @"[^a-zA-Z0-9]*$", "");
                try
                {
                    var port = new SerialPort(fixedName, 9600, Parity.None, 8, StopBits.One);
                    port.Open();
                    ports.Add(port);
                }
                catch (IOException e)
                {
                    CoreConfig.Logger.LogDebug($"Cannot open port {fixedName}: {e.Message}");
                }
            }
            HandleCoroutine(Run());
        }

        private void OnDestroy()
        {
            ports.ForEach(port => port.Close());
            ports.Clear();
        }

        private IEnumerator Run()
        {
            yield return HandleCoroutine(FindHotdog());
            yield return HandleCoroutine(PollHotdog());
        }

        private IEnumerator FindHotdog()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.1f);
                foreach (var port in ports)
                {
                    string line = GetLastLine(port);
                    if (!line.StartsWith(ProtocolId))
                    {
                        continue;
                    }
                    CoreConfig.Logger.LogInfo($"Found Hotdog on port {port.PortName}");
                    ports.ForEach(p => p.Close());
                    hotdogPort = new SerialPort(port.PortName, port.BaudRate, port.Parity,
                        port.DataBits, port.StopBits);
                    hotdogPort.Open();
                    IsDeviceConnected = true;
                    yield break;
                }
            }
        }

        private IEnumerator PollHotdog()
        {
            while (hotdogPort.IsOpen)
            {
                yield return new WaitForEndOfFrame();
                string line = GetLastLine(hotdogPort);
                string[] lineParsed = line.Split(Separator);
                if (lineParsed.Length != 3 || lineParsed[1] != DepthKey)
                {
                    continue;
                }
                if (!float.TryParse(lineParsed[2], out float result))
                {
                    CoreConfig.Logger.LogWarning($"Malformed Hotdog output {line}");
                    continue;
                }
                depth = result;
            }
        }

        private string GetLastLine(SerialPort port)
        {
            string[] lines = port.ReadExisting().Split('\n');
            return lines.Where(line => line.Length > 0).LastOrDefault() ?? "";
        }
    }
}
