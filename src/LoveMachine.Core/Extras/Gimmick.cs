using System.Collections;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace LoveMachine.Core
{
    /// <summary>
    /// Extend this if the game you're modding has additional "buttplug-able"
    /// features (e.g. fondling in Koikatsu or spanking in HS2).
    /// </summary>
    public abstract class Gimmick : MonoBehaviour
    {
        protected HandleLevel SetLevel { get; private set; }
        protected HandleStroke DoStroke { get; private set; }

        /// <summary>
        /// Will be started by each device controller when an H-scene starts;
        /// do whatever needs to be done.
        /// </summary>
        [HideFromIl2Cpp]
        protected abstract IEnumerator Run(Device device);

        internal IEnumerator Run(Device device, HandleLevel handleLevel, HandleStroke handleStroke)
        {
            SetLevel = handleLevel;
            DoStroke = handleStroke;
            yield return Run(device);
        }
        
        protected internal delegate void HandleLevel(Device device, float level,
            float durationSecs);
        
        protected internal delegate IEnumerator HandleStroke(Device device, float durationSecs);
    }
}