using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoveMachine.Core.Config
{
    internal static class OscillateConfig
    {
        public static ConfigEntry<ConstrictMode> Mode { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            const string constrictSettingsTitle = "Oscillate Settings";
            Mode = plugin.Config.Bind(
               section: constrictSettingsTitle,
               key: "Oscillate Mode",
               defaultValue: ConstrictMode.Cycle,
               new ConfigDescription(
                   "Cycle: repeat building up and releasing oscillation over a set duration\n" +
                   "Stroke Length: oscillation is based on the in-game stroke length\n" +
                   "Stroke Speed: oscillation is based on the in-game stroke speed",
                   tags: new ConfigurationManagerAttributes { Order = --order }));
        }

        public enum ConstrictMode
        {
            StrokeSpeed, Cycle, StrokeLength
        }
    }
}
