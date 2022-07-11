using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace LoveMachine.KK
{
    public static class KKExperimentalConfig
    {
        public static ConfigEntry<bool> EnableCalorDepthControl { get; private set; }
        public static ConfigEntry<bool> EnableHotdogDepthControl { get; private set; }
        public static ConfigEntry<string> HotdogServerAddress { get; private set; }

        public static void Initialize(BaseUnityPlugin plugin)
        {
            string experimentalTitle = "Experimental Features";
            string experimentalNote = "These settings will only take effect after " +
                "restarting the game. It is recommended that you close Intiface when " +
                "using any of these features.";
            plugin.Config.Bind(
                section: experimentalTitle,
                key: "a",
                defaultValue: 0,
                new ConfigDescription(
                    "",
                    new AcceptableValueRange<int>(0, 100),
                    new ConfigurationManagerAttributes
                    {
                        Order = 100,
                        CustomDrawer = entry => GUILayout.Label(experimentalNote),
                        HideSettingName = true,
                        HideDefaultButton = true
                    })
                );
            EnableCalorDepthControl = plugin.Config.Bind(
                section: experimentalTitle,
                key: "Enable Lovense Calor depth control",
                defaultValue: false,
                "Use a Lovense Calor device for depth control");
            EnableHotdogDepthControl = plugin.Config.Bind(
                section: experimentalTitle,
                key: "Enable Hotdog depth control",
                defaultValue: false,
                "Use a Hotdog device for depth control");
            HotdogServerAddress = plugin.Config.Bind(
                section: experimentalTitle,
                key: "Hotdog server address",
                defaultValue: "ws://localhost:5365",
                "The address of the Hotdog server");
        }
    }
}
