using System;
using System.Collections;
using BepInEx.Logging;

namespace LoveMachine.Core.Util
{
    internal static class CoroutineUtil
    {
        public static IEnumerator HandleExceptions(IEnumerator coroutine, bool suppressExceptions,
            ManualLogSource logger)
        {
            while (TryNext(coroutine, suppressExceptions, logger))
            {
                yield return coroutine.Current;
            }
        }

        private static bool TryNext(IEnumerator coroutine, bool suppressExceptions,
            ManualLogSource logger)
        {
            try
            {
                return coroutine.MoveNext();
            }
            catch (Exception e)
            {
                logger.LogError($"Coroutine failed with exception: {e}");
                if (suppressExceptions)
                {
                    return false;
                }
                throw;
            }
        }
    }
}