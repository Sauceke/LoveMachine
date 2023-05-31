using LoveMachine.Core;
using System;
using System.Collections;
using UnityEngine;

namespace LoveMachine.KK
{
    internal class KoikatsuAnimationController : ButtplugController
    {
        protected override bool IsDeviceSupported(Device device) =>
            throw new NotImplementedException();

        protected override IEnumerator Run(Device device) =>
            throw new NotImplementedException();

        protected override IEnumerator Run()
        {
            var kk = gameObject.GetComponent<KoikatsuGame>();
            while (true)
            {
                if (KKAnimationConfig.SuppressAnimationBlending.Value)
                {
                    kk.Flags.curveMotion = new AnimationCurve(new Keyframe());
                }
                yield return new WaitForSeconds(.5f);
            }
        }
    }
}