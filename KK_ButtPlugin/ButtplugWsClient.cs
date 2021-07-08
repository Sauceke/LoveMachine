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
            websocket.Opened += new EventHandler(OnOpened);
            websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(OnMessageReceived);
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
            ButtPlugin.Logger.LogDebug("Message from Buttplug server: " + e.Message);
        }
    }
}
