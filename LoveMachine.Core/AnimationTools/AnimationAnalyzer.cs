using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoveMachine.Core
{
    public abstract class AnimationAnalyzer : CoroutineHandler
    {
        // exact pose -> fractional part of normalized time at start of up-stroke
        private static Dictionary<string, float> animPhases = new Dictionary<string, float>();

        // exact pose -> number of strokes per loop
        private static Dictionary<string, int> animFreqs = new Dictionary<string, int>();

        protected abstract int AnimationLayer { get; }
        protected abstract Animator GetFemaleAnimator(int girlIndex);
        protected abstract List<Transform> GetFemaleBones(int girlIndex);
        protected abstract Transform GetMaleBone();
        protected abstract string GetPose(int girlIndex);

        protected AnimatorStateInfo GetAnimatorStateInfo(int girlIndex) =>
            GetFemaleAnimator(girlIndex).GetCurrentAnimatorStateInfo(AnimationLayer);

        private string GetExactPose(int girlIndex, int boneIndex)
            => GetPose(girlIndex) + "." + boneIndex;

        protected bool TryGetPhase(int girlIndex, int boneIndex, out float phase)
        {
            string pose = GetExactPose(girlIndex, 0);
            float placeholder = -1f;
            if (!animPhases.TryGetValue(pose, out phase))
            {
                animPhases[pose] = placeholder; // avoid multiple interleaving calls
                HandleCoroutine(ComputeAnimationOffsets(girlIndex));
                return false;
            }
            return animPhases.TryGetValue(GetExactPose(girlIndex, boneIndex), out phase)
                && phase != placeholder;
        }

        protected int GetStrokesPerAnimationCycle(int girlIndex, int boneIndex)
        {
            animFreqs.TryGetValue(GetExactPose(girlIndex, boneIndex), out int strokes);
            return strokes == 0 ? 1 : strokes;
        }

        protected virtual float GetStrokeTimeSecs(int girlIndex, int boneIndex)
        {
            var info = GetAnimatorStateInfo(girlIndex);
            float strokeTimeSecs = info.length / info.speed
                / GetStrokesPerAnimationCycle(girlIndex, boneIndex);
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

        private IEnumerator ComputeAnimationOffsets(int girlIndex)
        {
            var boneM = GetMaleBone();
            var femaleBones = GetFemaleBones(girlIndex);
            string pose = GetExactPose(girlIndex, 0);
            var measurements = new List<Measurement>();
            yield return new WaitForSeconds(0.1f);
            float currentTime = GetAnimatorStateInfo(girlIndex).normalizedTime;
            while (GetAnimatorStateInfo(girlIndex).normalizedTime - 1 < currentTime)
            {
                yield return new WaitForEndOfFrame();
                for (int i = 0; i < femaleBones.Count; i++)
                {
                    var boneF = femaleBones[i];
                    float distanceSq = (boneM.position - boneF.position).sqrMagnitude;
                    measurements.Add(new Measurement
                    {
                        BoneIndex = i,
                        Time = GetAnimatorStateInfo(girlIndex).normalizedTime,
                        DistanceSq = distanceSq
                    });
                }
            }
            if (pose != GetExactPose(girlIndex, 0))
            {
                CoreConfig.Logger.LogWarning($"Pose {pose} interrupted; canceling calibration.");
                animPhases.Remove(pose);
                yield break;
            }
            for (int i = 0; i < femaleBones.Count; i++)
            {
                animPhases[GetExactPose(girlIndex, i + 1)] = measurements
                    .Where(entry => entry.BoneIndex == i)
                    .OrderBy(entry => entry.DistanceSq)
                    .FirstOrDefault()
                    .Time % 1;
                animFreqs[GetExactPose(girlIndex, i + 1)] = GetFrequency(measurements
                    .Where(entry => entry.BoneIndex == i)
                    .OrderBy(entry => entry.Time)
                    .Select(entry => entry.DistanceSq));
            }
            var closest = measurements
                .OrderBy(entry => entry.DistanceSq)
                .FirstOrDefault();
            animPhases[GetExactPose(girlIndex, 0)] = closest.Time % 1;
            animFreqs[GetExactPose(girlIndex, 0)] =
                animFreqs[GetExactPose(girlIndex, closest.BoneIndex + 1)];
            CoreConfig.Logger.LogInfo($"Calibration for pose {pose} completed. " +
                $"{measurements.Count / femaleBones.Count} frames inspected. " +
                $"Closest bone index: {closest.BoneIndex}, offset: {closest.Time % 1}, " +
                $"frequency: {animFreqs[GetExactPose(girlIndex, 0)]}. ");
            CoreConfig.Logger.LogDebug(
                $"Raw measurement data for pose {pose}: {JsonUtility.ToJson(measurements)}");
        }

        private static int GetFrequency(IEnumerable<float> samples)
        {
            // catch flatlines
            if (samples.Max() - samples.Min() <= 0.000001f)
            {
                return 1;
            }
            // get frequency using Fourier series
            var dfsMagnitudes = new List<float>();
            // probably no game has more than 10 strokes in a loop
            for (int k = 1; k < 10; k++)
            {
                float freq = 2f * Mathf.PI / samples.Count() * k;
                float re = samples.Select((amp, index) => amp * Mathf.Cos(freq * index)).Sum();
                float im = samples.Select((amp, index) => amp * Mathf.Sin(freq * index)).Sum();
                dfsMagnitudes.Add(re * re + im * im);
            }
            return dfsMagnitudes.IndexOf(dfsMagnitudes.Max()) + 1;
        }

        internal void ClearMeasurements()
        {
            animPhases.Clear();
            animFreqs.Clear();
        }

        private struct Measurement
        {
            public int BoneIndex { get; set; }
            public float Time { get; set; }
            public float DistanceSq { get; set; }
        }
    }
}
