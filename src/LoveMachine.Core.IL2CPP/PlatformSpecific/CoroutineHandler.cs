using BepInEx.Unity.IL2CPP.Utils.Collections;
using System.Collections;
using BepInEx.Logging;
using Il2CppInterop.Runtime.Injection;
using LoveMachine.Core.Util;
using UnityEngine;

namespace LoveMachine.Core.PlatformSpecific;

public class CoroutineHandler : MonoBehaviour
{
    protected static ManualLogSource Logger => Globals.Logger;
        
    protected Coroutine HandleCoroutine(IEnumerator coroutine,
        bool suppressExceptions = false) =>
        StartCoroutine(CoroutineUtil.HandleExceptions(coroutine, suppressExceptions, Logger)
            .WrapToIl2Cpp());

    protected new T[] GetComponents<T>()
        where T : MonoBehaviour
    {
        ClassInjector.RegisterTypeInIl2Cpp<T>();
        return base.GetComponents<T>();
    }
}