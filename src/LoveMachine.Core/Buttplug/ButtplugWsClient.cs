using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using LoveMachine.Core.Config;
using LoveMachine.Core.PlatformSpecific;
using UnityEngine;
using WebSocket4Net;

namespace LoveMachine.Core.Buttplug
{
    public class ButtplugWsClient : CoroutineHandler
    {
        private WebSocket websocket;
        private bool killSwitchThrown = false;
        private ConcurrentQueue<IEnumerator> incoming;

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
            incoming = new ConcurrentQueue<IEnumerator>();
            string address = ButtplugConfig.WebSocketHost.Value
                + ":" + ButtplugConfig.WebSocketPort.Value;
            Logger.LogInfo($"Connecting to Intiface server at {address}");
            websocket = new WebSocket(address);
            // StartCoroutine is only safe to call inside Unity's main thread
            websocket.Opened += (s, e) => incoming.Enqueue(OnOpened());
            websocket.MessageReceived += (s, e) => incoming.Enqueue(OnMessageReceived(e));
            websocket.Error += (s, e) => incoming.Enqueue(OnError(e));
            websocket.Open();
            HandleCoroutine(RunReceiveLoop());
            HandleCoroutine(RunKillSwitchLoop());
            HandleCoroutine(RunBatteryLoop());
        }

        public void Close()
        {
            StopAllCoroutines();
            IsConnected = false;
            Logger.LogInfo("Disconnecting from Intiface server.");
            websocket.Close();
            websocket.Dispose();
        }

        public void LinearCmd(Device device, float position, float durationSecs) =>
            SendKillable(Buttplug.LinearCmd(device, position, durationSecs));

        public void VibrateCmd(Device device, float intensity) =>
            SendKillable(Buttplug.ScalarCmd(device, intensity, Buttplug.Feature.Vibrate));

        public void ConstrictCmd(Device device, float pressure) =>
            SendKillable(Buttplug.ScalarCmd(device, pressure, Buttplug.Feature.Constrict));

        public void RotateCmd(Device device, float speed, bool clockwise) =>
            SendKillable(Buttplug.RotateCmd(device, speed, clockwise));

        public void BatteryLevelCmd(Device device) => Send(Buttplug.BatteryLevelCmd(device));

        public void StopDeviceCmd(Device device) => Send(Buttplug.StopDeviceCmd(device));

        public void StopAllDevices() => Send(Buttplug.StopAllDevices());

        private void RequestServerInfo() => Send(Buttplug.RequestServerInfo());

        private void RequestDeviceList() => Send(Buttplug.RequestDeviceList());

        public void StartScan() => Send(Buttplug.StartScan());

        private void StopScan() => Send(Buttplug.StopScan());

        public void Connect()
        {
            Close(); // close previous connection just in case
            Open();
        }

        private void Send(object command) => websocket.Send(JsonMapper.ToJson(new[] { command }));

        private void SendKillable(object command)
        {
            if (!killSwitchThrown)
            {
                Send(command);
            }
        }

        private IEnumerator OnOpened()
        {
            Logger.LogInfo("Successfully connected to Intiface.");
            RequestServerInfo();
            yield break;
        }

        private IEnumerator OnMessageReceived(MessageReceivedEventArgs e)
        {
            foreach (JsonData data in JsonMapper.ToObject(e.Message))
            {
                bool _ = CheckOkMsg(data)
                    || CheckErrorMsg(data)
                    || CheckServerInfoMsg(data)
                    || CheckDeviceAddedRemovedMsg(data)
                    || CheckDeviceListMsg(data)
                    || CheckBatteryLevelReadingMsg(data);
            }
            yield break;
        }

        private IEnumerator OnError(SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Logger.LogWarning($"Websocket error: {e.Exception.Message}");
            if (!IsConnected)
            {
                Logger.LogMessage("Error: Failed to connect to Intiface.");
            }
            yield break;
        }

        private bool CheckOkMsg(JsonData data) => data.ContainsKey("Ok");
        
        private bool CheckErrorMsg(JsonData data)
        {
            if (!data.ContainsKey("Error"))
            {
                return false;
            }
            Logger.LogWarning($"Error from Intiface: {data.ToJson()}");
            return true;
        }

        private bool CheckServerInfoMsg(JsonData data)
        {
            if (!data.ContainsKey("ServerInfo"))
            {
                return false;
            }
            IsConnected = true;
            Logger.LogInfo("Handshake successful.");
            StartScan();
            RequestDeviceList();
            return true;
        }

        private bool CheckDeviceAddedRemovedMsg(JsonData data)
        {
            if (!data.ContainsKey("DeviceAdded") && !data.ContainsKey("DeviceRemoved"))
            {
                return false;
            }
            RequestDeviceList();
            return true;
        }

        private bool CheckDeviceListMsg(JsonData data)
        {
            if (!data.ContainsKey("DeviceList"))
            {
                return false;
            }
            var previousDevices = Devices;
            Devices = JsonMapper.ToObject<Buttplug.DeviceListMessage<Device>>(data.ToJson())
                .DeviceList.Devices;
            var args = new DeviceListEventArgs(before: previousDevices, after: Devices);
            OnDeviceListUpdated.Invoke(this, args);
            ReadBatteryLevels();
            return true;
        }

        private bool CheckBatteryLevelReadingMsg(JsonData data)
        {
            var reading = JsonMapper.ToObject<Buttplug.SensorReadingMessage>(data.ToJson());
            if (reading.SensorReading?.SensorType != Buttplug.Feature.Battery)
            {
                return false;
            }
            float level = reading.SensorReading.Data[0] / 100f;
            int index = reading.SensorReading.DeviceIndex;
            Devices.Where(device => device.DeviceIndex == index).ToList()
                .ForEach(device => device.BatteryLevel = level);
            return true;
        }

        private void ReadBatteryLevels() =>
            Devices.Where(device => device.HasBatteryLevel).ToList().ForEach(BatteryLevelCmd);

        private IEnumerator RunReceiveLoop()
        {
            while (true)
            {
                while (incoming.TryDequeue(out var coroutine))
                {
                    HandleCoroutine(coroutine);
                }
                yield return new WaitForSecondsRealtime(1f);
            }
        }

        private IEnumerator RunKillSwitchLoop()
        {
            while (true)
            {
                yield return null;
                killSwitchThrown &= !KillSwitchConfig.ResumeSwitch.Value.IsPressed();
                if (KillSwitchConfig.KillSwitch.Value.IsDown())
                {
                    Logger.LogMessage("LoveMachine: Emergency stop pressed.");
                    StopAllDevices();
                    killSwitchThrown = true;
                }
            }
        }

        private IEnumerator RunBatteryLoop()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(60f);
                ReadBatteryLevels();
                Devices
                    .Where(device => device.BatteryLevel > 0f && device.BatteryLevel < 0.2f)
                    .Select(device => $"{device.DeviceName}: battery low.")
                    .ToList().ForEach(Logger.LogMessage);
            }
        }

        internal class DeviceListEventArgs : EventArgs
        {
            public List<Device> Before { get; }
            public List<Device> After { get; }

            public DeviceListEventArgs(List<Device> before, List<Device> after)
            {
                Before = before;
                After = after;
            }
        }
    }
}