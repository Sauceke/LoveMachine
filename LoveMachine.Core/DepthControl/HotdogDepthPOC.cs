﻿using System;
using LitJson;
using WebSocket4Net;

namespace LoveMachine.Core
{
    public class HotdogDepthPOC : DepthPOC
    {
        private WebSocket websocket;

        private void Start()
        {
            string address = CoreConfig.HotdogServerAddress.Value;
            websocket = new WebSocket(address);
            websocket.Opened += OnOpened;
            websocket.MessageReceived += OnMessageReceived;
            websocket.Error += (s, a) => { };
            websocket.Open();
        }

        private void OnDestroy()
        {
            websocket.Close();
            websocket.Dispose();
        }

        private void OnOpened(object sender, EventArgs e)
        {
            CoreConfig.Logger.LogInfo("Connected to Hotdog server.");
            IsDeviceConnected = true;
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Depth = 1 - JsonMapper.ToObject<DepthData>(e.Message).Depth;
        }

        private struct DepthData
        {
            public float Depth { get; set; }
        }
    }
}
