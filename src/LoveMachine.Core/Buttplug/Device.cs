using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LoveMachine.Core.Device;

namespace LoveMachine.Core
{
    public class Device
    {
        public string DeviceName
        {
            get => Settings.DeviceName;
            set => Settings.DeviceName = value;
        }

        public int DeviceIndex { get; set; }
        public float BatteryLevel { get; set; }
        public DeviceSettings Settings { get; set; } = new DeviceSettings();
        public Features DeviceMessages { get; set; }

        public bool IsVibrator => DeviceMessages.ScalarCmd?.Any(f => f.IsVibrator) ?? false;
        public bool IsStroker => DeviceMessages.LinearCmd != null;
        public bool IsRotator => DeviceMessages.RotateCmd != null;

        public bool HasBatteryLevel =>
            DeviceMessages.SensorReadCmd?.Any(f => f.HasBatteryLevel) ?? false;

        public class Features
        {
            public Feature[] LinearCmd { get; set; }
            public Feature[] ScalarCmd { get; set; }
            public Feature[] RotateCmd { get; set; }
            public Feature[] SensorReadCmd { get; set; }

            public class Feature
            {
                internal const string Vibrate = "Vibrate";
                internal const string Battery = "Battery";

                public string ActuatorType { get; set; }
                public string SensorType { get; set; }

                public bool IsVibrator => ActuatorType == Vibrate;
                public bool HasBatteryLevel => SensorType == Battery;
            }
        }

        internal bool Matches(DeviceSettings settings) => settings.DeviceName == DeviceName;

        internal void Draw()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(DeviceName);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            if (HasBatteryLevel)
            {
                GUIUtil.SingleSpace();
                GUILayout.BeginHorizontal();
                {
                    GUIUtil.PercentBar("Battery", "Current battery level.", BatteryLevel);
                }
                GUILayout.EndHorizontal();
            }
            GUIUtil.SingleSpace();
            GUILayout.BeginHorizontal();
            {
                GUIUtil.LabelWithTooltip("Features", "What this device can do.");
                GUILayout.Toggle(IsStroker, "Stroker");
                GUILayout.Toggle(IsVibrator, "Vibrator");
                GUILayout.Toggle(IsRotator, "Rotator");
            }
            GUILayout.EndHorizontal();
            GUIUtil.SingleSpace();
            Settings.Draw();
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