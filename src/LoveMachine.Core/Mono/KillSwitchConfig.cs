using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace LoveMachine.Core
{
    internal static class KillSwitchConfig
    {
        public static ConfigEntry<KeyboardShortcut> KillSwitch { get; private set; }
        public static ConfigEntry<KeyboardShortcut> ResumeSwitch { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            const string killSwitchSettingsTitle = "Kill Switch Settings";
            KillSwitch = plugin.Config.Bind(
                section: killSwitchSettingsTitle,
                key: "Emergency Stop Key Binding",
                defaultValue: new KeyboardShortcut(KeyCode.Space),
                "Shortcut to stop all devices immediately.");
            ResumeSwitch = plugin.Config.Bind(
                section: killSwitchSettingsTitle,
                key: "Resume Key Binding",
                defaultValue: new KeyboardShortcut(KeyCode.F8),
                "Shortcut to resume device activities.");
        }
    }
}