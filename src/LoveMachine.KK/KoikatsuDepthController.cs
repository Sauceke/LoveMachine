using LoveMachine.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoveMachine.KK
{
    internal class KoikatsuDepthController<T> : ButtplugController
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

        private bool IsControllable => supportedAnimations.Contains(kk.Flags.nowAnimStateName);

        private bool IsPenetrable => penetrableAnimations.Contains(kk.Flags.nowAnimStateName);

        protected override IEnumerator Run()
        {
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
                if (!analyzer.TryGetWaveInfo(0, Bone.Auto, out var waveInfo))
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
            kk.Flags.isCondom = true;
            do
            {
                kk.Flags.click = HFlag.ClickKind.insert;
                yield return new WaitForSeconds(1f);
            }
            while (IsPenetrable);
            do
            {
                kk.Flags.click = HFlag.ClickKind.modeChange;
                kk.Flags.speedCalc = 0.5f;
                yield return new WaitForSeconds(1f);
            }
            while (!kk.Flags.nowAnimStateName.Contains("WLoop"));
            kk.Flags.click = HFlag.ClickKind.motionchange;
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator HandleDepth(AnimationAnalyzer.WaveInfo waveInfo)
        {
            SetSpeed(0f);
            float startNormTime = kk.GetFemaleAnimator(0)
                .GetCurrentAnimatorStateInfo(kk.AnimationLayer)
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
            kk.GetFemaleAnimator(0).speed = speed;
            kk.Flags.player.chaCtrl.animBody.speed = speed;
        }

        private void SkipToTime(float normalizedTime)
        {
            int animStateHash = kk.GetAnimatorStateInfo(0).fullPathHash;
            kk.GetFemaleAnimator(0).Play(animStateHash, kk.AnimationLayer, normalizedTime);
            kk.Flags.player.chaCtrl.animBody
                .Play(animStateHash, kk.AnimationLayer, normalizedTime);
        }
    }

    internal class KoikatsuCalorDepthController : KoikatsuDepthController<CalorDepthPOC>
    { }

    internal class KoikatsuHotdogDepthController : KoikatsuDepthController<HotdogDepthPOC>
    { }
}