using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using WebSocket4Net;

namespace KK_ButtPlugin
{
    public class ButtplugWsClient
    {
        private WebSocket websocket;
        private readonly Random random = new Random();
        private List<Device> devices; 

        public ButtplugWsClient()
        {
            Open();
        }

        public void Open()
        {
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
            ButtPlugin.Logger.LogDebug("Disconnecting from Buttplug server.");
            websocket.Close();
            websocket.Dispose();
        }

        public void LinearCmd(double position, int durationMs)
        {
            var commands = (
                from device in devices
                where device.DeviceMessages.LinearCmd != null
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
            websocket.Send(JsonMapper.ToJson(commands));
        }

        public void VibrateCmd(double intensity)
        {
            var commands = (
                from device in devices
                where device.DeviceMessages.VibrateCmd != null
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
            websocket.Send(JsonMapper.ToJson(commands));
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
                    devices = JsonMapper.ToObject<DeviceListMessage>(data.ToJson())
                        .DeviceList.Devices;
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

    internal class Device
    {
        public int DeviceIndex { get; set; }
        public Features DeviceMessages { get; set; }

        internal class Features
        {
            public Command LinearCmd { get; set; }
            public Command VibrateCmd { get; set; }

            internal class Command
            {
                public int FeatureCount { get; set; }
            }
        }
    }
}
