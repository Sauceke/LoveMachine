using BepInEx;
using BepInEx.Configuration;
using LoveMachine.Core.Common;
using LoveMachine.Core.NonPortable;

namespace LoveMachine.Core.Config
{
    public class CoreConfig
    {
        public static ConfigEntry<POV> POV { get; private set; }

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            int order = 1000;
            const string coreSettingsTitle = "Core Settings";
            POV = plugin.Config.Bind(
                section: coreSettingsTitle,
                key: "POV",
                defaultValue: Common.POV.Balanced,
                new ConfigDescription(
                    "Which point of view to simulate.",
                    tags: new ConfigurationManagerAttributes { Order = --order }));
        }
    }
}