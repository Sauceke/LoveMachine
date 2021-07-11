using LitJson;
using System;
using System.Collections.Generic;
using WebSocket4Net;

namespace KK_ButtPlugin
{
    public class ButtplugWsClient
    {
        private WebSocket websocket;
        private readonly Random random = new Random();

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
            var command = new
            {
                LinearCmd = new
                {
                    Id = random.Next(),
                    DeviceIndex = 0,
                    Vectors = new object[]
                    {
                        new
                        {
                            Index = 0,
                            Duration = durationMs,
                            Position = position
                        }
                    }
                }
            };
            websocket.Send(JsonMapper.ToJson(new List<object> { command }));
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

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            foreach(JsonData data in JsonMapper.ToObject(e.Message))
            {
                if (data.ContainsKey("Error"))
                {
                    ButtPlugin.Logger.LogWarning("Buttplug error: " + data.ToJson());
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
}
