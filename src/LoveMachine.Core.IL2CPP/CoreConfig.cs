using BepInEx.Logging;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace LoveMachine.Core
{
    public static class CoreConfig
    {
        public const string PluginName = "LoveMachine IL2CPP";
        public const string GUID = "Sauceke.LoveMachine.IL2CPP";
        public const string Version = VersionInfo.Version;

        public static ManualLogSource Logger { get; private set; }
        public static Manager ManagerObject { get; } = new();

        internal static void Initialize(ManualLogSource logger) => Logger = logger;

        public class Manager
        {
            private GameObject go;

            public T AddComponent<T>()
                where T : MonoBehaviour
            {
                ClassInjector.RegisterTypeInIl2Cpp<T>();
                if (go == null)
                {
                    go = new GameObject("LoveMachineManager");
                    go.hideFlags = HideFlags.HideAndDontSave;
                    UnityEngine.Object.DontDestroyOnLoad(go);
                }
                return go.AddComponent<T>();
            }

            public T GetComponent<T>()
                where T : MonoBehaviour => go.GetComponent<T>();
        }
    }
}