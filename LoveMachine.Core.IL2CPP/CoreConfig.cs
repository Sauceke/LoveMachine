using BepInEx.Logging;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace LoveMachine.Core
{
    public static class CoreConfig
    {
        private static Manager manager = new Manager();

        public const string PluginName = "LoveMachine";
        public const string GUID = "Sauceke.LoveMachine";
        public const string Version = VersionInfo.Version;

        public static ManualLogSource Logger { get; private set; }
        public static Manager ManagerObject => manager;

        internal static void Initialize(BaseUnityPlugin plugin, ManualLogSource logger)
        {
            Logger = logger;
        }

        public class Manager
        {
            public T AddComponent<T>()
                where T : MonoBehaviour
            {
                ClassInjector.RegisterTypeInIl2Cpp<T>();
                return GameObject.Find("LoveMachineManager").AddComponent<T>();
            }

            public T GetComponent<T>()
                where T : MonoBehaviour => GameObject.Find("LoveMachineManager").GetComponent<T>();
        }
    }
}
