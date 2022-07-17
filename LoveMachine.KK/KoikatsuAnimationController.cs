using System;
using System.Collections;
using System.Linq;
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
            foreach (int girlIndex in Enumerable.Range(0, kk.Flags.lstHeroine.Count))
            {
                HandleCoroutine(Run(girlIndex));
            }
            yield break;
        }

        protected IEnumerator Run(int girlIndex)
        {
            var animator = kk.GetFemaleAnimator(girlIndex);
            var playerAnimator = kk.Flags.player.chaCtrl.animBody;
            while (!kk.Flags.isHSceneEnd)
            {
                var info = animator.GetCurrentAnimatorStateInfo(0);
                if (KKAnimationConfig.ReduceAnimationSpeeds.Value)
                {
                    // nerf the animation speed so the device can keep up with it
                    // OLoop is faster than the rest, about 280ms per stroke at its original speed
                    NerfAnimationSpeeds(info.IsName("OLoop") ? 0.28f : 0.375f,
                        animator, playerAnimator);
                }
                if (KKAnimationConfig.SuppressAnimationBlending.Value)
                {
                    kk.Flags.curveMotion = new AnimationCurve(new Keyframe[] { new Keyframe() });
                }
                yield return new WaitForSeconds(.5f);
            }
        }
    }
}
