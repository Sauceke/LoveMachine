using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoveMachine.Core
{
    internal static class KillSwitchConfig
    {
        public static class ResumeSwitch
        {
            public static class Value
            {
                public static bool IsPressed() => false;
            }
        }

        public static class KillSwitch
        {
            public static class Value
            {
                public static bool IsDown() => false;
            }
        }

        public static void Initialize(BaseUnityPlugin plugin) { }
    }
}
