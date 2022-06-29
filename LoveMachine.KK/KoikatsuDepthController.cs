using System.Collections;
using System.Collections.Generic;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.KK
{
    internal class KoikatsuDepthController<T> : KoikatsuButtplugController
        where T : DepthPOC
    {
        private static readonly List<string> supportedAnimations = new List<string>
        {
            "WLoop", "SLoop",
            // anal
            "A_WLoop", "A_SLoop", "A_OLoop"
        };

        private static readonly List<string> penetrableAnimations = new List<string>
        {
            "Idle", "OUT_A"
        };

        private T depthSensor;

        private bool IsControllable => supportedAnimations.Contains(flags.nowAnimStateName);

        private bool IsPenetrable => penetrableAnimations.Contains(flags.nowAnimStateName);

        protected override void HandleFondle(float y, int girlIndex, Bone bone, float timeSecs) =>
            throw new System.NotImplementedException();

        protected override bool IsIdle(int girlIndex) =>
            throw new System.NotImplementedException();

        protected override IEnumerator Run(int girlIndex, Bone bone)
        {
            if (girlIndex != 0 || bone != Bone.Auto)
            {
                yield break;
            }
            depthSensor = gameObject.GetComponent<T>();
            if (depthSensor == null)
            {
                CoreConfig.Logger.LogInfo($"{GetType()} is disabled.");
                yield break;
            }
            while (true)
            {
                yield return new WaitForEndOfFrame();
                if (!depthSensor.IsDeviceConnected)
                {
                    yield return new WaitForSecondsRealtime(1f);
                    continue;
                }
                if (IsPenetrable)
                {
                    CoreConfig.Logger.LogInfo("Penetrable position found. Inserting.");
                    yield return HandleCoroutine(Penetrate());
                    continue;
                }
                if (!TryGetWaveInfo(0, Bone.Auto, out var waveInfo))
                {
                    SetSpeed(1f);
                    yield return new WaitForSecondsRealtime(0.1f);
                    continue;
                }
                if (!IsControllable)
                {
                    SetSpeed(1f);
                    continue;
                }
                HandleCoroutine(HandleDepth(waveInfo));
            }
        }

        private IEnumerator Penetrate()
        {
            flags.isCondom = true;
            do
            {
                flags.click = HFlag.ClickKind.insert;
                yield return new WaitForSeconds(1f);
            }
            while (IsPenetrable);
            do
            {
                flags.click = HFlag.ClickKind.modeChange;
                flags.speedCalc = 0.5f;
                yield return new WaitForSeconds(1f);
            }
            while (!flags.nowAnimStateName.Contains("WLoop"));
            flags.click = HFlag.ClickKind.motionchange;
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator HandleDepth(WaveInfo waveInfo)
        {
            SetSpeed(0f);
            float startNormTime = GetFemaleAnimator(0)
                .GetCurrentAnimatorStateInfo(AnimationLayer)
                .normalizedTime;
            float depth = depthSensor.Depth;
            float targetNormTime = waveInfo.Phase + 0.5f / waveInfo.Frequency - depth / 2f;
            float delta = targetNormTime - startNormTime;
            float step = Mathf.Sign(delta) / 30f;
            int steps = (int)(delta / step);
            for (int i = 1; i <= steps; i++)
            {
                SkipToTime(startNormTime + step * i);
                yield return new WaitForEndOfFrame();
                if (depthSensor.Depth != depth)
                {
                    yield break;
                }
            }
        }

        private void SetSpeed(float speed)
        {
            GetFemaleAnimator(0).speed = speed;
            flags.player.chaCtrl.animBody.speed = speed;
        }

        private void SkipToTime(float normalizedTime)
        {
            int animStateHash = GetAnimatorStateInfo(0).fullPathHash;
            GetFemaleAnimator(0).Play(animStateHash, AnimationLayer, normalizedTime);
            flags.player.chaCtrl.animBody.Play(animStateHash, AnimationLayer, normalizedTime);
        }
    }

    internal class KoikatsuCalorDepthController : KoikatsuDepthController<CalorDepthPOC> { }

    internal class KoikatsuHotdogDepthController : KoikatsuDepthController<HotdogDepthPOC> { }
}
