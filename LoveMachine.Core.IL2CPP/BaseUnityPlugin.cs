using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;

namespace LoveMachine.Core
{
    public class BaseUnityPlugin
    {
        private BasePlugin plugin;

        public ConfigFile Config => plugin.Config;
    }
}
