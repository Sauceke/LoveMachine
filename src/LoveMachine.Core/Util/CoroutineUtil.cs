using System;
using System.Collections;

namespace LoveMachine.Core
{
    internal static class CoroutineUtil
    {
        public static IEnumerator HandleExceptions(IEnumerator coroutine, bool suppressExceptions)
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
    }
}