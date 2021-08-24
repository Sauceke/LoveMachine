using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using UnityEngine;
using WebSocket4Net;

namespace ButtPlugin.Core
{
    public class ButtplugWsClient : MonoBehaviour
    {
        private WebSocket websocket;
        private readonly System.Random random = new System.Random();
        public List<Device> Devices { get; private set; }

        public bool IsConnected { get; private set; }

        private void Awake()
        {
            Open();
        }

        private void OnDestroy()
        {
            Close();
        }

        public void Open()
        {
            IsConnected = false;
            Devices = new List<Device>();
            string address = CoreConfig.WebSocketAddress.Value;
            CoreConfig.Logger.LogDebug($"Connecting to Buttplug server at {address}");
            websocket = new WebSocket(address);
            websocket.Opened += OnOpened;
            websocket.MessageReceived += OnMessageReceived;
            websocket.Error += OnError;
            websocket.Open();
            StartCoroutine(KillSwitch.RunLoop());
        }

        public void Close()
        {
            StopAllCoroutines();
            IsConnected = false;
            CoreConfig.Logger.LogDebug("Disconnecting from Buttplug server.");
            websocket.Close();
            websocket.Dispose();
            DeviceManager.SaveDeviceSettings(Devices);
        }

        public void LinearCmd(double position, int durationMs, int girlIndex, int actionIndex = 0)
        {
            if (KillSwitch.Pushed)
            {
                return;
            }
            var commands = (
                from device in Devices
                where device.IsStroker
                    && device.GirlIndex == girlIndex
                    && device.ActionIndex == actionIndex
                select new
                {
                    LinearCmd = new
                    {
                        Id = random.Next(),
                        DeviceIndex = device.DeviceIndex,
                        Vectors = (
                            from featureIndex in Enumerable.Range(0,
                                device.DeviceMessages.LinearCmd.FeatureCount)
                            select new
                            {
                                Index = featureIndex,
                                Duration = durationMs,
                                Position = position
                            }
                        ).ToList()
                    }
                }
            ).ToList();
            if (commands.Count > 0)
            {
                websocket.Send(JsonMapper.ToJson(commands));
            }
        }

        public void VibrateCmd(double intensity, int girlIndex, int actionIndex = 0)
        {
            if (KillSwitch.Pushed && intensity != 0f)
            {
                VibrateCmd(0f, girlIndex);
                return;
            }
            var commands = (
                from device in Devices
                where device.IsVibrator
                    && device.GirlIndex == girlIndex
                    && device.ActionIndex == actionIndex
                select new
                {
                    VibrateCmd = new
                    {
                        Id = random.Next(),
                        DeviceIndex = device.DeviceIndex,
                        Speeds = (
                            from featureIndex in Enumerable.Range(0,
                                device.DeviceMessages.VibrateCmd.FeatureCount)
                            select new
                            {
                                Index = featureIndex,
                                Speed = intensity
                            }
                        ).ToList()
                    }
                }
            ).ToList();
            if (commands.Count > 0)
            {
                websocket.Send(JsonMapper.ToJson(commands));
            }
        }

        private void OnOpened(object sender, EventArgs e)
        {
            CoreConfig.Logger.LogDebug("Succesfully connected.");
            var handshake = new
            {
                RequestServerInfo = new
                {
                    Id = random.Next(),
                    ClientName = "Koikatsu",
                    MessageVersion = 1
                }
            };
            websocket.Send(JsonMapper.ToJson(new object[] { handshake }));
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
            websocket.Send(JsonMapper.ToJson(new object[] { deviceListRequest }));
        }

        private void StartScan()
        {
            var scanRequest = new
            {
                StartScanning = new
                {
                    Id = random.Next()
                }
            };
            websocket.Send(JsonMapper.ToJson(new object[] { scanRequest }));
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
            websocket.Send(JsonMapper.ToJson(new object[] { scanRequest }));
        }

        private IEnumerator ScanDevices()
        {
            StartScan();
            yield return new WaitForSeconds(30.0f);
            StopScan();
        }

        public void Scan()
        {
            StartCoroutine(ScanDevices());
        }

        public void Connect()
        {
            Close(); // close previous connection just in case
            Open();
            Scan();
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            foreach (JsonData data in JsonMapper.ToObject(e.Message))
            {
                if (data.ContainsKey("Error"))
                {
                    CoreConfig.Logger.LogWarning($"Buttplug error: {data.ToJson()}");
                }
                else if (data.ContainsKey("ServerInfo") || data.ContainsKey("DeviceAdded")
                    || data.ContainsKey("DeviceRemoved"))
                {
                    Scan();
                    RequestDeviceList();
                }
                else if (data.ContainsKey("DeviceList"))
                {
                    Devices = JsonMapper.ToObject<DeviceListMessage>(data.ToJson())
                        .DeviceList.Devices;
                    DeviceManager.LoadDeviceSettings(Devices);
                    LogDevices();
                }

                if (data.ContainsKey("ServerInfo"))
                {
                    IsConnected = true;
                    CoreConfig.Logger.LogDebug("Handshake successful.");
                }
            }
        }

        private void OnError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            CoreConfig.Logger.LogWarning($"Websocket error: {e.Exception.Message}");
            if (e.Exception.Message.Contains("unreachable"))
            {
                CoreConfig.Logger.LogMessage("Error: Failed to connect to Buttplug server.");
            }
        }

        private void LogDevices()
        {
            CoreConfig.Logger.LogDebug($"List of devices: {JsonMapper.ToJson(Devices)}");
            if (Devices.Count == 0)
            {
                CoreConfig.Logger.LogMessage("Warning: No devices connected to Buttplug.");
            }
            foreach (var device in Devices)
            {
                if (!device.IsStroker && !device.IsVibrator)
                {
                    CoreConfig.Logger.LogMessage(
                        $"Warning: device \"{device.DeviceName}\" not supported.");
                }
            }
        }

        private static class KillSwitch
        {
            public static bool Pushed { get; private set; }

            public static IEnumerator RunLoop()
            {
                while (true)
                {
                    Pushed &= !CoreConfig.ResumeSwitch.Value.IsPressed();
                    if (CoreConfig.KillSwitch.Value.IsDown())
                    {
                        CoreConfig.Logger.LogMessage("ButtPlugin: Emergency stop pressed.");
                        Pushed = true;
                    }
                    yield return null;
                }
            }
        }
    }
}
