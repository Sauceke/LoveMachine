using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public class CoroutineHandler : MonoBehaviour
    {
        protected internal Coroutine HandleCoroutine(IEnumerator coroutine,
            bool suppressExceptions = false) =>
            StartCoroutine(HandleExceptions(coroutine, suppressExceptions).WrapToIl2Cpp());

        private static IEnumerator HandleExceptions(IEnumerator coroutine, bool suppressExceptions)
        {
            while (TryNext(coroutine, suppressExceptions))
            {
                yield return coroutine.Current;
            }
        }

        private static bool TryNext(IEnumerator coroutine, bool suppressExceptions)
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

        // couldn't figure out how to convert Func<> to Il2CppSystem.Func<>
        // oh well
        private static IEnumerator _WaitWhile(Func<bool> condition)
        {
            while (condition())
            {
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }

        public Coroutine WaitWhile(Func<bool> condition) => HandleCoroutine(_WaitWhile(condition));

        public Coroutine WaitUntil(Func<bool> condition) => WaitWhile(() => !condition());
    }
}
