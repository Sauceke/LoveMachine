using System.Linq;
using UnityEngine;

namespace LoveMachine.Core
{
    public class Device : Buttplug.Device
    {
        public override string DeviceName
        {
            get => Settings.DeviceName;
            set => Settings.DeviceName = value;
        }

        public float BatteryLevel { get; set; }
        public DeviceSettings Settings { get; set; } = new DeviceSettings();

        public bool IsSupported => IsVibrator || IsConstrictor || IsStroker || IsRotator;
        public bool IsVibrator => DeviceMessages.ScalarCmd?.Any(f => f.IsVibrator) ?? false;
        public bool IsConstrictor => DeviceMessages.ScalarCmd?.Any(f => f.IsConstrictor) ?? false;
        public bool IsStroker => DeviceMessages.LinearCmd != null;
        public bool IsRotator => DeviceMessages.RotateCmd != null;

        public bool HasBatteryLevel =>
            DeviceMessages.SensorReadCmd?.Any(f => f.HasBatteryLevel) ?? false;

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
                GUILayout.Toggle(IsStroker, "Position");
                GUILayout.Toggle(IsVibrator, "Vibration");
                GUILayout.Toggle(IsRotator, "Rotation");
                GUILayout.Toggle(IsConstrictor, "Pressure");
            }
            GUILayout.EndHorizontal();
            GUIUtil.SingleSpace();
            Settings.Draw();
        }
    }
}