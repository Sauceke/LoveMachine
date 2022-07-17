using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using LitJson;
using UnityEngine;
using WebSocket4Net;

namespace LoveMachine.Core
{
    public class ButtplugWsClient : MonoBehaviour
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
            Close();
        }

        public void Open()
        {
            IsConnected = false;
            Devices = new List<Device>();
            string address = ButtplugConfig.WebSocketAddress.Value;
            CoreConfig.Logger.LogInfo($"Connecting to Intiface server at {address}");
            websocket = new WebSocket(address);
            websocket.Opened += OnOpened;
            websocket.MessageReceived += OnMessageReceived;
            websocket.Error += OnError;
            websocket.Open();
            StartCoroutine(RunKillSwitchLoop());
        }

        public void Close()
        {
            StopAllCoroutines();
            IsConnected = false;
            CoreConfig.Logger.LogInfo("Disconnecting from Intiface server.");
            websocket.Close();
            websocket.Dispose();
            DeviceManager.SaveDeviceSettings(Devices);
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
                            Duration = (int)(durationSecs * 1000),
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

        private void OnOpened(object sender, EventArgs e)
        {
            CoreConfig.Logger.LogInfo("Succesfully connected to Intiface.");
            var handshake = new
            {
                RequestServerInfo = new
                {
                    Id = random.Next(),
                    ClientName = Paths.ProcessName,
                    MessageVersion = 1
                }
            };
            SendSingleCommand(handshake);
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

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
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
                    Devices = JsonMapper.ToObject<DeviceListMessage>(data.ToJson())
                        .DeviceList.Devices;
                    DeviceManager.LoadDeviceSettings(Devices);
                    LogDevices();
                    OnDeviceListUpdated.Invoke(this, new DeviceListEventArgs());
                }
                if (data.ContainsKey("ServerInfo"))
                {
                    IsConnected = true;
                    CoreConfig.Logger.LogInfo("Handshake successful.");
                    StartScan();
                }
            }
        }

        private void OnError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
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
    }
}
