using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Controller;
using LoveMachine.Core.Game;
using LoveMachine.Core.UI.Util;
using UnityEngine;

namespace LoveMachine.Core.UI.Extensions
{
    internal static class DeviceUIExtension
    {
        public static void Draw(this Device device, GameAdapter game,
            ClassicButtplugController[] controllers)
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
                foreach (var controller in controllers)
                {
                    GUILayout.Toggle(controller.IsDeviceSupported(device), controller.FeatureName);
                }
            }
            GUILayout.EndHorizontal();
            GUIUtil.SingleSpace();
            device.Settings.Draw(game);
        }
    }
}