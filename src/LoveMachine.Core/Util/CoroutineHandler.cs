using System;
using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public class CoroutineHandler : MonoBehaviour
    {
        protected internal Coroutine HandleCoroutine(IEnumerator coroutine,
            bool suppressExceptions = false) =>
            StartCoroutine(HandleExceptions(coroutine, suppressExceptions));

        private IEnumerator HandleExceptions(IEnumerator coroutine, bool suppressExceptions)
        {
            while (TryNext(coroutine, suppressExceptions))
            {
                yield return coroutine.Current;
            }
        }

        private bool TryNext(IEnumerator coroutine, bool suppressExceptions)
        {
            try
            {
                return coroutine.MoveNext();
            }
            catch (Exception e)
            {
                CoreConfig.Logger.LogError($"Coroutine failed with exception: {e}");
                if (suppressExceptions)
                {
                    return false;
                }
                throw;
            }
        }

        public CustomYieldInstruction WaitWhile(Func<bool> condition) => new WaitWhile(condition);

        public CustomYieldInstruction WaitUntil(Func<bool> condition) => new WaitUntil(condition);
    }
}