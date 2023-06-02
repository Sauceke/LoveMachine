using System;
using System.Collections;
using BepInEx.Logging;
using UnityEngine;

namespace LoveMachine.Core
{
    public class CoroutineHandler : MonoBehaviour
    {
        protected static ManualLogSource Logger => Globals.Logger;
        
        protected Coroutine HandleCoroutine(IEnumerator coroutine,
            bool suppressExceptions = false) =>
            StartCoroutine(CoroutineUtil.HandleExceptions(coroutine, suppressExceptions));

        protected CustomYieldInstruction WaitWhile(Func<bool> condition) =>
            new WaitWhile(condition);

        protected CustomYieldInstruction WaitUntil(Func<bool> condition) =>
            new WaitUntil(condition);
    }
}