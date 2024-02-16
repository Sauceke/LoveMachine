using System.Collections;
using BepInEx.Logging;
using LoveMachine.Core.Common;
using UnityEngine;

namespace LoveMachine.Core.NonPortable
{
    public class CoroutineHandler : MonoBehaviour
    {
        protected static ManualLogSource Logger => Globals.Logger;
        
        protected Coroutine HandleCoroutine(IEnumerator coroutine,
            bool suppressExceptions = false) =>
            StartCoroutine(CoroutineUtil.HandleExceptions(coroutine, suppressExceptions, Logger));
    }
}