using System.Collections;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.KK
{
    internal class KoikatsuAnimationController : ButtplugController
    {
        private KoikatsuGame game;

        private void Start() => game = gameObject.GetComponent<KoikatsuGame>();

        protected override IEnumerator Run(int girlIndex, Bone bone)
        {
            var animator = game.GetFemaleAnimator(girlIndex);
            var playerAnimator = game.Flags.player.chaCtrl.animBody;
            while (!game.Flags.isHSceneEnd)
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
                    game.Flags.curveMotion = new AnimationCurve(new Keyframe[] { new Keyframe() });
                }
                yield return new WaitForSeconds(.5f);
            }
        }

        protected override void StopDevices(int girlIndex, Bone bone) { }
    }
}
