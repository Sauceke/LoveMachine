using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public class StrokerController : ButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone)
        {
            while (true)
            {
                if (game.IsIdle(girlIndex) || !IsEnabled)
                {
                    yield return new WaitForSeconds(.1f);
                    continue;
                }
                if (game.IsOrgasming(girlIndex))
                {
                    yield return HandleCoroutine(EmulateOrgasm(girlIndex, bone));
                    continue;
                }
                yield return HandleCoroutine(EmulateStroking(girlIndex, bone));
            }
        }

        protected override void StopDevices(int girlIndex, Bone bone) { }

        protected virtual bool IsEnabled => !CoreConfig.SmoothStroking.Value;

        protected virtual IEnumerator EmulateStroking(int girlIndex, Bone bone)
        {
            string pose = game.GetPose(girlIndex);
            yield return WaitForUpStroke(girlIndex, bone);
            if (game.GetPose(girlIndex) != pose)
            {
                yield break;
            }
            float strokeTimeSecs = GetStrokeTimeSecs(girlIndex, bone);
            analyzer.TryGetWaveInfo(girlIndex, bone, out var waveInfo);
            float relativeLength = (waveInfo.Crest - waveInfo.Trough) / game.PenisSize;
            float scale = Mathf.Lerp(1f - CoreConfig.StrokeLengthRealism.Value, 1f,
                relativeLength);
            for (int i = 0; i < waveInfo.Frequency - 1; i++)
            {
                HandleCoroutine(DoStroke(girlIndex, bone, strokeTimeSecs, scale));
                yield return new WaitForSecondsRealtime(strokeTimeSecs);
            }
            yield return HandleCoroutine(DoStroke(girlIndex, bone, strokeTimeSecs, scale));
        }

        private void GetStrokeZone(float strokeTimeSecs, float scale, out float min, out float max)
        {
            float minSlow = Mathf.InverseLerp(0, 100, CoreConfig.SlowStrokeZoneMin.Value);
            float maxSlow = Mathf.InverseLerp(0, 100, CoreConfig.SlowStrokeZoneMax.Value);
            float minFast = Mathf.InverseLerp(0, 100, CoreConfig.FastStrokeZoneMin.Value);
            float maxFast = Mathf.InverseLerp(0, 100, CoreConfig.FastStrokeZoneMax.Value);
            // decrease stroke length gradually as speed approaches the device limit
            float rate = 60f / CoreConfig.MaxStrokesPerMinute.Value / strokeTimeSecs;
            min = Mathf.Lerp(minSlow, minFast, rate) * scale;
            max = Mathf.Lerp(maxSlow, maxFast, rate) * scale;
        }

        protected virtual float GetStrokeTimeSecs(int girlIndex, Bone bone)
        {
            game.GetAnimState(girlIndex, out _, out float length, out float speed);
            int freq = analyzer.TryGetWaveInfo(girlIndex, bone, out var result)
                ? result.Frequency
                : 1;
            float strokeTimeSecs = length / speed / freq;
            // sometimes the length of an animation becomes Infinity in KK
            // sometimes the speed becomes 0 in HS2
            // this is a catch-all for god knows what other things that can
            // possibly go wrong and cause the stroking coroutine to hang
            if (strokeTimeSecs > 10 || strokeTimeSecs < 0.001f
                || float.IsNaN(strokeTimeSecs))
            {
                return .01f;
            }
            return strokeTimeSecs;
        }

        protected CustomYieldInstruction WaitForUpStroke(int girlIndex, Bone bone)
        {
            float normalizedTime()
            {
                game.GetAnimState(girlIndex, out float time, out _, out _);
                return time;
            }
            string startPose = game.GetPose(girlIndex);
            float startNormTime = normalizedTime();
            float strokeTimeSecs = GetStrokeTimeSecs(girlIndex, bone);
            float latencyNormTime = CoreConfig.LatencyMs.Value / 1000f / strokeTimeSecs;
            bool timeToStroke() => game.GetPose(girlIndex) != startPose
                || (analyzer.TryGetWaveInfo(girlIndex, bone, out var result)
                    && (int)(normalizedTime() - result.Phase + latencyNormTime + 10f)
                        != (int)(startNormTime - result.Phase + latencyNormTime + 10f));
            return new WaitUntil(timeToStroke);
        }

        protected internal IEnumerator DoStroke(int girlIndex, Bone bone,
            float strokeTimeSecs, float scale = 1f, bool forceHard = false)
        {
            float hardness = forceHard || game.IsHardSex
                ? Mathf.InverseLerp(0, 100, CoreConfig.HardSexIntensity.Value)
                : 0;
            float downStrokeTimeSecs = Mathf.Lerp(strokeTimeSecs / 2f, strokeTimeSecs / 4f,
                hardness);
            GetStrokeZone(strokeTimeSecs, scale, out float min, out float max);
            MoveStroker(
                position: max,
                durationSecs: strokeTimeSecs / 2f - 0.01f,
                girlIndex,
                bone);
            // needs to be real time so we can test devices even when the game is paused
            yield return new WaitForSecondsRealtime(strokeTimeSecs * 0.75f -
                downStrokeTimeSecs / 2f);
            MoveStroker(
                position: min,
                durationSecs: downStrokeTimeSecs - 0.01f,
                girlIndex,
                bone);
        }

        protected IEnumerator EmulateOrgasm(int girlIndex, Bone bone)
        {
            float bottom = CoreConfig.OrgasmDepth.Value;
            float time = 0.5f / CoreConfig.OrgasmShakingFrequency.Value;
            float top = bottom + CoreConfig.MaxStrokesPerMinute.Value / 60f / 2 * time;
            while (game.IsOrgasming(girlIndex))
            {
                MoveStroker(top, time, girlIndex, bone);
                yield return new WaitForSecondsRealtime(time);
                MoveStroker(bottom, time, girlIndex, bone);
                yield return new WaitForSecondsRealtime(time);

            }
        }

        protected void MoveStroker(float position, float durationSecs, int girlIndex, Bone bone) =>
            client.LinearCmd(position, durationSecs, girlIndex, bone);
    }
}
