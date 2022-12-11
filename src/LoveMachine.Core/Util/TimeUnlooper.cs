using UnityEngine;

namespace LoveMachine.Core
{
    public class TimeUnlooper
    {
        private float lastLoopingTime = 0f;
        private int totalTime = 0;

        public float LoopingToMonotonic(float loopingTime)
        {
            if (loopingTime < lastLoopingTime)
            {
                totalTime += Mathf.CeilToInt(lastLoopingTime - loopingTime);
            }
            lastLoopingTime = loopingTime;
            return loopingTime + totalTime;
        }
    }
}