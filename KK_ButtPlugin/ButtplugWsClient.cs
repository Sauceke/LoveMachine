using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WebSocket4Net;
using UnityEngine;

namespace KK_ButtPlugin
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
            string address = ButtPlugin.WebSocketAddress.Value;
            ButtPlugin.Logger.LogDebug("Connecting to Buttplug server at " + address);
            websocket = new WebSocket(address);
            websocket.Opened += OnOpened;
            websocket.MessageReceived += OnMessageReceived;
            websocket.Error += OnError;
            websocket.Open();
        }

        public void Close()
        {
            IsConnected = false;
            ButtPlugin.Logger.LogDebug("Disconnecting from Buttplug server.");
            websocket.Close();
            websocket.Dispose();            
        }

        public void LinearCmd(double position, int durationMs, int girlIndex)
        {
            var commands = (
                from device in Devices
                where device.IsStroker && device.GirlIndex == girlIndex
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
            if (!commands.IsNullOrEmpty())
            {
                websocket.Send(JsonMapper.ToJson(commands));
            }
        }

        public void VibrateCmd(double intensity)
        {
            var commands = (
                from device in Devices
                where device.IsVibrator
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
            if (!commands.IsNullOrEmpty())
            {
                websocket.Send(JsonMapper.ToJson(commands));
            }
        }

        private void OnOpened(object sender, EventArgs e)
        {
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
            yield return new WaitForSeconds(15.0f);
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
                    ButtPlugin.Logger.LogWarning("Buttplug error: " + data.ToJson());
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
                    ButtPlugin.Logger.LogDebug(JsonMapper.ToJson(Devices));
                }

                if (data.ContainsKey("ServerInfo"))
                {
                    IsConnected = true;
                }
            }
        }

        private void OnError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            ButtPlugin.Logger.LogWarning("Websocket error: " + e.Exception.Message);
            if (e.Exception.Message.Contains("unreachable"))
            {
                ButtPlugin.Logger.LogMessage("Error: Failed to connect to Buttplug server");
            }
        }
    }

    internal class DeviceListMessage
    {
        public DeviceListWrapper DeviceList { get; set; }

        internal class DeviceListWrapper
        {
            public List<Device> Devices { get; set; }
        }
    }

    public class Device
    {
        public string DeviceName { get; set; }
        public int DeviceIndex { get; set; }
        public int GirlIndex { get; set; } = 0;
        public Features DeviceMessages { get; set; }

        public bool IsVibrator { get { return DeviceMessages.VibrateCmd != null; } }
        public bool IsStroker { get { return DeviceMessages.LinearCmd != null; } }

        public class Features
        {
            public Command LinearCmd { get; set; }
            public Command VibrateCmd { get; set; }

            public class Command
            {
                public int FeatureCount { get; set; }
            }
        }
    }
}
