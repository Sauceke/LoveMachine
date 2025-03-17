using System.Collections;
using LoveMachine.Core.Buttplug;
using LoveMachine.Core.NonPortable;
using UnityEngine;

namespace LoveMachine.Core.Controller.Addons
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
        protected abstract IEnumerator Run(DeviceFeature feature);

        internal IEnumerator Run(DeviceFeature feature, HandleLevel handleLevel, HandleStroke handleStroke)
        {
            SetLevel = handleLevel;
            DoStroke = handleStroke;
            yield return Run(feature);
        }
        
        protected internal delegate void HandleLevel(DeviceFeature feature, float level,
            float durationSecs);
        
        protected internal delegate IEnumerator HandleStroke(DeviceFeature feature, float durationSecs);
    }
}