using System;
using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public class CoroutineHandler : MonoBehaviour
    {
        protected Coroutine HandleCoroutine(IEnumerator coroutine,
            bool suppressExceptions = false) =>
            StartCoroutine(CoroutineUtil.HandleExceptions(coroutine, suppressExceptions));

        public CustomYieldInstruction WaitWhile(Func<bool> condition) => new WaitWhile(condition);

        public CustomYieldInstruction WaitUntil(Func<bool> condition) => new WaitUntil(condition);
    }
}