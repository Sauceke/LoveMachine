using BepInEx;
using BepInEx.Logging;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace LoveMachine.Core.PlatformSpecific;

public static class Globals
{
    public const string PluginName = "LoveMachine IL2CPP";
    public const string GUID = "Sauceke.LoveMachine.IL2CPP";
    public const string Version = VersionInfo.Version;

    public static string PluginPath { get; private set; }
    public static ManualLogSource Logger { get; private set; }
    public static Manager ManagerObject { get; private set; }

    internal static void Initialize(ManualLogSource logger, PluginInfo info)
    {
        Logger = logger;
        PluginPath = Path.GetDirectoryName(info.Location);
        ManagerObject = new();
    }

    public class Manager
    {
        private readonly GameObject go;

        internal Manager()
        {
            go = new GameObject("LoveMachineManager");
            go.hideFlags = HideFlags.HideAndDontSave;
            UnityEngine.Object.DontDestroyOnLoad(go);
        }
            
        public T AddComponent<T>()
            where T : MonoBehaviour
        {
            ClassInjector.RegisterTypeInIl2Cpp<T>();
            return go.AddComponent<T>();
        }

        public T GetComponent<T>()
            where T : MonoBehaviour => go.GetComponent<T>();

        protected T[] GetComponents<T>()
            where T : MonoBehaviour
        {
            ClassInjector.RegisterTypeInIl2Cpp<T>();
            return go.GetComponents<T>();
        }
    }
}