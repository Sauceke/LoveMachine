using System;
using System.Collections;
using BepInEx.IL2CPP.Utils.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public class CoroutineHandler : MonoBehaviour
    {
        public CoroutineHandler(IntPtr handle) : base(handle) { }

        protected internal Coroutine HandleCoroutine(IEnumerator coroutine,
            bool suppressExceptions = false) =>
            StartCoroutine(HandleExceptions(coroutine, suppressExceptions).WrapToIl2Cpp());

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
                CoreConfig.Logger.LogError((string)$"Coroutine failed with exception: {e}");
                if (suppressExceptions)
                {
                    return false;
                }
                throw;
            }
        }
    }
}
