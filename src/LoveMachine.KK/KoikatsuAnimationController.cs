using System;
using System.Collections;
using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Controller;
using UnityEngine;

namespace LoveMachine.KK
{
    internal class KoikatsuAnimationController : ButtplugController
    {
        public override bool IsDeviceSupported(Device device) =>
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