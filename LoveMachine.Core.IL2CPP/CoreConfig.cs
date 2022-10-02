using System.IO;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
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
            private GameObject go;

            public T AddComponent<T>()
                where T : MonoBehaviour
            {
                if (go == null)
                {
                    go = new GameObject { hideFlags = HideFlags.HideAndDontSave };
                }
                if (!ClassInjector.IsTypeRegisteredInIl2Cpp<T>())
                {
                    ClassInjector.RegisterTypeInIl2Cpp<T>();
                }
                return go.AddComponent<T>();
            }

            public T GetComponent<T>()
                where T : MonoBehaviour => go.GetComponent<T>();
        }
    }
}
