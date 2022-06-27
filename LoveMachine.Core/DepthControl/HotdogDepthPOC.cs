using System;
using LitJson;
using UnityEngine;
using WebSocket4Net;

namespace LoveMachine.Core
{
    public class HotdogDepthPOC : CoroutineHandler, IDepthSensor
    {
        private float? depth = null;

        public bool IsDeviceConnected { get; private set; } = false;

        public bool TryGetNewDepth(bool peek, out float newDepth)
        {
            if (!depth.HasValue)
            {
                newDepth = 0f;
                return false;
            }
            newDepth = depth > 0.9f ? -1f : Mathf.InverseLerp(0.9f, 0f, depth.Value);
            depth = peek ? depth : null;
            return true;
        }

        private void Start()
        {
            string address = "ws://localhost:5365";
            var websocket = new WebSocket(address);
            websocket.Opened += OnOpened;
            websocket.MessageReceived += OnMessageReceived;
            websocket.Error += (s, a) => { };
            websocket.Open();
        }

        private void OnOpened(object sender, EventArgs e)
        {
            CoreConfig.Logger.LogInfo("Connected to Hotdog server.");
            IsDeviceConnected = true;
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            depth = JsonMapper.ToObject<DepthData>(e.Message).Depth;
        }

        private struct DepthData
        {
            public float Depth { get; set; }
        }
    }
}
