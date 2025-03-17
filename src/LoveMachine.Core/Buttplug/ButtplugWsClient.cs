using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using LoveMachine.Core.Config;
using LoveMachine.Core.NonPortable;
using UnityEngine;
using WebSocket4Net;

namespace LoveMachine.Core.Buttplug
{
    internal class ButtplugWsClient : CoroutineHandler
    {
        private WebSocket websocket;
        private ConcurrentQueue<IEnumerator> incoming;

        public event EventHandler<DeviceListEventArgs> OnDeviceListUpdated;

        public List<Device> Devices { get; private set; }

        public bool IsConnected { get; private set; }

        public bool IsConsensual { get; set; } = true;

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
            websocket.Closed += (s, e) => incoming.Enqueue(OnClosed());
            websocket.MessageReceived += (s, e) => incoming.Enqueue(OnMessageReceived(e));
            websocket.Error += (s, e) => incoming.Enqueue(OnError(e));
            websocket.Open();
            HandleCoroutine(RunReceiveLoop());
        }

        public void Close()
        {
            Logger.LogInfo("Disconnecting from Intiface server.");
            websocket.Close();
            CleanUp();
        }

        public void LinearCmd(DeviceFeature feature, float position, float durationSecs) =>
            SendWithConsent(
                Buttplug.LinearCmd(feature.Device, feature.FeatureIndex, position, durationSecs),
                feature);

        public void VibrateCmd(DeviceFeature feature, float intensity) =>
            SendWithConsent(
                Buttplug.ScalarCmd(feature.Device, feature.FeatureIndex, intensity,
                    Buttplug.Feature.Vibrate),
                feature);

        public void ConstrictCmd(DeviceFeature feature, float pressure) =>
            SendWithConsent(
                Buttplug.ScalarCmd(feature.Device, feature.FeatureIndex, pressure,
                    Buttplug.Feature.Constrict),
                feature);

        public void RotateCmd(DeviceFeature feature, float speed, bool clockwise) =>
            SendWithConsent(
                Buttplug.RotateCmd(feature.Device, feature.FeatureIndex, speed, clockwise),
                feature);

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

        private void SendWithConsent(object command, DeviceFeature feature)
        {
            if (IsConsensual && feature.Settings.Enabled)
            {
                Send(command);
            }
        }

        private void CleanUp()
        {
            websocket.Dispose();
            StopAllCoroutines();
            if (Devices.Any())
            {
                UpdateDeviceList(new List<Device>());
            }
            IsConnected = false;
        }

        private IEnumerator OnOpened()
        {
            Logger.LogInfo("Connected to Intiface. Commencing handshake.");
            RequestServerInfo();
            yield break;
        }

        private IEnumerator OnClosed()
        {
            Logger.LogInfo(IsConnected
                ? "Disconnected from Intiface."
                : "Failed to connect to Intiface.");
            CleanUp();
            HandleCoroutine(Reconnect());
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
            yield break;
        }

        private IEnumerator Reconnect()
        {
            int retrySecs = ButtplugConfig.ReconnectBackoffSecs.Value;
            Logger.LogInfo($"Attempting to reconnect in {retrySecs} seconds...");
            yield return new WaitForSecondsRealtime(retrySecs);
            Open();
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
            if (IsConnected)
            {
                Logger.LogWarning("Ignoring handshake message, client already registered.");
                return true;
            }
            IsConnected = true;
            Logger.LogInfo("Handshake successful.");
            StartScan();
            RequestDeviceList();
            HandleCoroutine(RunBatteryLoop());
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
            UpdateDeviceList(JsonMapper.ToObject<Buttplug.DeviceListMessage<Device>>(data.ToJson())
                .DeviceList.Devices);
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

        private void UpdateDeviceList(List<Device> newDevices)
        {
            var oldDevices = Devices;
            Devices = newDevices;
            var args = new DeviceListEventArgs(before: oldDevices, after: Devices);
            OnDeviceListUpdated.Invoke(this, args);
        }

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

        public class DeviceListEventArgs : EventArgs
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