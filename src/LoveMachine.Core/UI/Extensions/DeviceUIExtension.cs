using LoveMachine.Core.Buttplug;
using LoveMachine.Core.UI.Util;
using UnityEngine;

namespace LoveMachine.Core.UI.Extensions
{
    internal static class DeviceUIExtension
    {
        public static void Draw(this Device device)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(device.DeviceName);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            if (device.HasBatteryLevel)
            {
                GUIUtil.SingleSpace();
                GUILayout.BeginHorizontal();
                {
                    GUIUtil.PercentBar("Battery", "Current battery level.", device.BatteryLevel);
                }
                GUILayout.EndHorizontal();
            }
            GUIUtil.SingleSpace();
            GUILayout.BeginHorizontal();
            {
                GUIUtil.LabelWithTooltip("Features", "What this device can do.");
                GUILayout.Toggle(device.IsStroker, "Position");
                GUILayout.Toggle(device.IsVibrator, "Vibration");
                GUILayout.Toggle(device.IsRotator, "Rotation");
                GUILayout.Toggle(device.IsConstrictor, "Pressure");
                GUILayout.Toggle(device.IsOscillate, "Oscillation");
            }
            GUILayout.EndHorizontal();
            GUIUtil.SingleSpace();
            device.Settings.Draw();
        }
    }
}