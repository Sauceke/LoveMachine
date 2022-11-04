using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;

namespace LoveMachine.Core
{
    public class BaseUnityPlugin
    {
        private BasePlugin plugin;

        public BaseUnityPlugin(BasePlugin plugin) => this.plugin = plugin;

        public ConfigFile Config => plugin.Config;
    }
}