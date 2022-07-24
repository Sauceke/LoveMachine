using System;
using System.Collections;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.KK
{
    internal class KoikatsuAnimationController : ButtplugController
    {
        private KoikatsuGame kk;

        protected override bool IsDeviceSupported(Device device) =>
            throw new NotImplementedException();

        protected override IEnumerator Run(Device device) =>
            throw new NotImplementedException();

        protected override void Start()
        {
            base.Start();
            kk = gameObject.GetComponent<KoikatsuGame>();
        }

        protected override IEnumerator Run()
        {
            while (!kk.Flags.isHSceneEnd)
            {
                if (KKAnimationConfig.SuppressAnimationBlending.Value)
                {
                    kk.Flags.curveMotion = new AnimationCurve(new Keyframe[] { new Keyframe() });
                }
                yield return new WaitForSeconds(.5f);
            }
        }
    }
}
