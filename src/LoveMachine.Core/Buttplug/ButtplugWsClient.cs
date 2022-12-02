using BepInEx;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WebSocket4Net;

namespace LoveMachine.Core
{
    public class ButtplugWsClient : CoroutineHandler
    {
        private WebSocket websocket;
        private readonly System.Random random = new System.Random();
        private bool killSwitchThrown = false;

        internal event EventHandler<DeviceListEventArgs> OnDeviceListUpdated;

        public List<Device> Devices { get; private set; }

        public bool IsConnected { get; private set; }

        private void Start() => Open();

        private void OnDestroy()
        {
            StopScan();
            StopAllDevices();
            Close();
        }

        public void Open()
        {
            IsConnected = false;
            Devices = new List<Device>();
            string address = ButtplugConfig.WebSocketHost.Value
                + ":" + ButtplugConfig.WebSocketPort.Value;
            CoreConfig.Logger.LogInfo($"Connecting to Intiface server at {address}");
            websocket = new WebSocket(address);
            websocket.Opened += (s, e) => HandleCoroutine(OnOpened(s, e));
            websocket.MessageReceived += (s, e) => HandleCoroutine(OnMessageReceived(s, e));
            websocket.Error += (s, e) => HandleCoroutine(OnError(s, e));
            websocket.Open();
            HandleCoroutine(RunKillSwitchLoop());
            HandleCoroutine(RunBatteryLoop());
        }

        public void Close()
        {
            StopAllCoroutines();
            IsConnected = false;
            CoreConfig.Logger.LogInfo("Disconnecting from Intiface server.");
            websocket.Close();
            websocket.Dispose();
        }

        public void LinearCmd(Device device, double position, float durationSecs)
        {
            if (killSwitchThrown)
            {
                return;
            }
            var command = new
            {
                LinearCmd = new
                {
                    Id = random.Next(),
                    DeviceIndex = device.DeviceIndex,
                    Vectors = Enumerable.Range(0, device.DeviceMessages.LinearCmd.FeatureCount)
                        .Select(featureIndex => new
                        {
                            Index = featureIndex,
                            Duration = (int)(durationSecs * 1000f),
                            Position = position
                        })
                        .ToArray()
                }
            };
            SendSingleCommand(command);
        }

        public void VibrateCmd(Device device, double intensity)
        {
            if (killSwitchThrown)
            {
                return;
            }
            var command = new
            {
                VibrateCmd = new
                {
                    Id = random.Next(),
                    DeviceIndex = device.DeviceIndex,
                    Speeds = Enumerable.Range(0, device.DeviceMessages.VibrateCmd.FeatureCount)
                        .Select(featureIndex => new
                        {
                            Index = featureIndex,
                            Speed = intensity
                        })
                        .ToArray()
                }
            };
            SendSingleCommand(command);
        }

        public void RotateCmd(Device device, float speed, bool clockwise)
        {
            if (killSwitchThrown)
            {
                return;
            }
            var command = new
            {
                RotateCmd = new
                {
                    Id = random.Next(),
                    DeviceIndex = device.DeviceIndex,
                    Rotations = Enumerable.Range(0, device.DeviceMessages.RotateCmd.FeatureCount)
                        .Select(featureIndex => new
                        {
                            Index = featureIndex,
                            Speed = speed,
                            Clockwise = clockwise
                        })
                        .ToArray()
                }
            };
            SendSingleCommand(command);
        }

        public void BatteryLevelCmd(Device device)
        {
            var command = new
            {
                BatteryLevelCmd = new
                {
                    Id = random.Next(),
                    DeviceIndex = device.DeviceIndex
                }
            };
            SendSingleCommand(command);
        }

        public void StopDeviceCmd(Device device)
        {
            var command = new
            {
                StopDeviceCmd = new
                {
                    Id = random.Next(),
                    DeviceIndex = device.DeviceIndex
                }
            };
            SendSingleCommand(command);
        }

        public void StopAllDevices()
        {
            var command = new
            {
                StopAllDevices = new
                {
                    Id = random.Next()
                }
            };
            SendSingleCommand(command);
        }

        private void RequestDeviceList()
        {
            var deviceListRequest = new
            {
                RequestDeviceList = new
                {
                    Id = random.Next()
                }
            };
            SendSingleCommand(deviceListRequest);
        }

        public void StartScan()
        {
            var scanRequest = new
            {
                StartScanning = new
                {
                    Id = random.Next()
                }
            };
            SendSingleCommand(scanRequest);
        }

        private void StopScan()
        {
            var scanRequest = new
            {
                StopScanning = new
                {
                    Id = random.Next()
                }
            };
            SendSingleCommand(scanRequest);
        }

        public void Connect()
        {
            Close(); // close previous connection just in case
            Open();
        }

        private void SendSingleCommand(object command) =>
            websocket.Send(JsonMapper.ToJson(new[] { command }));

        private IEnumerator OnOpened(object sender, EventArgs e)
        {
            yield return new WaitForEndOfFrame();
            CoreConfig.Logger.LogInfo("Succesfully connected to Intiface.");
            var handshake = new
            {
                RequestServerInfo = new
                {
                    Id = random.Next(),
                    ClientName = Paths.ProcessName,
                    MessageVersion = 2
                }
            };
            SendSingleCommand(handshake);
        }

        private IEnumerator OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            yield return new WaitForEndOfFrame();
            foreach (JsonData data in JsonMapper.ToObject(e.Message))
            {
                if (data.ContainsKey("Error"))
                {
                    CoreConfig.Logger.LogWarning($"Error from Intiface: {data.ToJson()}");
                }
                else if (data.ContainsKey("ServerInfo") || data.ContainsKey("DeviceAdded")
                    || data.ContainsKey("DeviceRemoved"))
                {
                    RequestDeviceList();
                }
                else if (data.ContainsKey("DeviceList"))
                {
                    var previousDevices = Devices;
                    Devices = JsonMapper.ToObject<DeviceListMessage>(data.ToJson())
                        .DeviceList.Devices;
                    var args = new DeviceListEventArgs(before: previousDevices, after: Devices);
                    OnDeviceListUpdated.Invoke(this, args);
                    LogDevices();
                    HandleCoroutine(ReadBatteryLevels(retries: 5));
                }
                else if (data.ContainsKey("BatteryLevelReading"))
                {
                    var reading = JsonMapper.ToObject<BatteryLevelMessage>(data.ToJson())
                        .BatteryLevelReading;
                    Devices.Where(device => device.DeviceIndex == reading.DeviceIndex).ToList()
                        .ForEach(device => device.BatteryLevel = reading.BatteryLevel);
                }
                if (data.ContainsKey("ServerInfo"))
                {
                    IsConnected = true;
                    CoreConfig.Logger.LogInfo("Handshake successful.");
                    StartScan();
                }
            }
        }

        private IEnumerator OnError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            yield return new WaitForEndOfFrame();
            CoreConfig.Logger.LogWarning($"Websocket error: {e.Exception.Message}");
            if (e.Exception.Message.Contains("unreachable"))
            {
                CoreConfig.Logger.LogMessage("Error: Failed to connect to Intiface server.");
            }
        }

        private void LogDevices()
        {
            CoreConfig.Logger.LogInfo($"List of devices: {JsonMapper.ToJson(Devices)}");
            if (Devices.Count == 0)
            {
                CoreConfig.Logger.LogMessage("Warning: No devices connected to Intiface.");
            }
            else
            {
                CoreConfig.Logger.LogMessage($"{Devices.Count} device(s) connected to Intiface.");
            }
            foreach (var device in Devices)
            {
                if (!device.IsStroker && !device.IsVibrator && !device.IsRotator)
                {
                    CoreConfig.Logger.LogMessage(
                        $"Warning: device \"{device.DeviceName}\" not supported.");
                }
            }
        }

        private IEnumerator ReadBatteryLevels(int retries)
        {
            for (int i = 0; i < retries; i++)
            {
                Devices.Where(device => device.HasBatteryLevel).ToList()
                    .ForEach(BatteryLevelCmd);
                yield return new WaitForSecondsRealtime(1f);
            }
        }

        private IEnumerator RunKillSwitchLoop()
        {
            while (true)
            {
                killSwitchThrown &= !KillSwitchConfig.ResumeSwitch.Value.IsPressed();
                if (KillSwitchConfig.KillSwitch.Value.IsDown())
                {
                    CoreConfig.Logger.LogMessage("LoveMachine: Emergency stop pressed.");
                    StopAllDevices();
                    killSwitchThrown = true;
                }
                yield return null;
            }
        }

        private IEnumerator RunBatteryLoop()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(60f);
                yield return HandleCoroutine(ReadBatteryLevels(retries: 1));
                foreach (var device in Devices)
                {
                    if (device.BatteryLevel > 0f && device.BatteryLevel < 0.2f)
                    {
                        CoreConfig.Logger.LogMessage($"{device.DeviceName}: battery low.");
                    }
                }
            }
        }
    }
}