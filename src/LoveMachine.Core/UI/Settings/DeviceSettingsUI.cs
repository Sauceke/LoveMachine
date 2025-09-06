using System;
using System.Linq;
using System.Text.RegularExpressions;
using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using LoveMachine.Core.NonPortable;
using LoveMachine.Core.UI.Util;
using UnityEngine;

namespace LoveMachine.Core.UI.Settings
{
    internal class DeviceSettingsUI : SettingsUI
    {
        public override void Draw(DeviceSettings settings)
        {
            var defaults = new DeviceSettings();
            settings.LatencyMs = GUIUtil.IntSlider(
                label: "Latency (ms)",
                tooltip: "The difference in latency between this device and your display.\n" +
                         "Negative if this device has lower latency than your display.",
                value: settings.LatencyMs,
                defaultValue: defaults.LatencyMs,
                min: -500,
                max: 500);
            settings.UpdatesHz = GUIUtil.IntSlider(
                label: "Updates Per Second",
                tooltip: "Maximum number of commands this device can handle per second.",
                value: settings.UpdatesHz,
                defaultValue: defaults.UpdatesHz,
                min: 1,
                max: 30);
            if (settings.UpdatesHz != defaults.UpdatesHz)
            {
                GUIUtil.Warning("Updates Per Second should almost always be 10. " +
                    "Please make sure you know what you're doing.");
            }
        }
    }
}