using System.Collections;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace LoveMachine.Core
{
    public abstract class Gimmick : MonoBehaviour
    {
        private HandleLevel levelHandler;

        [HideFromIl2Cpp]
        protected abstract IEnumerator Run(Device device);

        internal IEnumerator Run(Device device, HandleLevel handleLevel)
        {
            levelHandler = handleLevel;
            yield return Run(device);
        }

        protected void SetLevel(Device device, float level, float durationSecs) =>
            levelHandler.Invoke(device, level, durationSecs);

        protected IEnumerator DoStroke(Device device, float durationSecs)
        {
            float halfDurationSecs = durationSecs / 2f;
            SetLevel(device, 1f, halfDurationSecs);
            yield return new WaitForSecondsRealtime(halfDurationSecs);
            SetLevel(device, 0f, halfDurationSecs);
        }

        internal delegate void HandleLevel(Device device, float level, float durationSecs);
    }
}