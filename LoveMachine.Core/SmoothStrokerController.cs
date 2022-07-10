using System.Collections;
using System.Linq;
using UnityEngine;

namespace LoveMachine.Core
{
    public class SmoothStrokerController : StrokerController
    {
        protected override bool IsEnabled => CoreConfig.SmoothStroking.Value;

        protected override IEnumerator EmulateStroking(int girlIndex, Bone bone)
        {
            if (!analyzer.TryGetWaveInfo(girlIndex, bone, out var waveInfo))
            {
                yield break;
            }
            game.GetAnimState(girlIndex, out float currentNormTime, out _, out _);
            var strokeTimeSecs = GetStrokeTimeSecs(girlIndex, bone);
            float refreshTimeSecs = 0.1f;
            float refreshNormTime = refreshTimeSecs / strokeTimeSecs;
            var currentSample = GetClosestSample(currentNormTime, waveInfo.Plot);
            var nextSample = GetClosestSample(currentNormTime + refreshNormTime, waveInfo.Plot);
            float currentPosition = GetNormalizedPosition(currentSample.Distance, waveInfo);
            float nextPosition = GetNormalizedPosition(nextSample.Distance, waveInfo);
            float speed = (nextPosition - currentPosition) / refreshTimeSecs;
            float targetPosition = currentPosition < nextPosition ? 1f : 0f;
            float timeToTargetSecs = (targetPosition - currentPosition) / speed;
            MoveStroker(targetPosition, timeToTargetSecs, girlIndex, bone);
            yield return new WaitForSecondsRealtime(refreshTimeSecs);
        }

        private AnimationAnalyzer.Sample GetClosestSample(float normalizedTime,
            AnimationAnalyzer.Sample[] samples) =>
            samples.OrderBy(sample => ((sample.Time - normalizedTime) % 1f + 1f) % 1f)
                .First();

        private float GetNormalizedPosition(float distance, AnimationAnalyzer.WaveInfo waveInfo) =>
            (distance - waveInfo.Trough) / (waveInfo.Crest - waveInfo.Trough);
    }
}
