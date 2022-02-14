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
        protected abstract Dictionary<Bone, Transform> GetFemaleBones(int girlIndex);
        protected abstract Transform GetMaleBone();
        protected abstract string GetPose(int girlIndex);

        protected AnimatorStateInfo GetAnimatorStateInfo(int girlIndex) =>
            GetFemaleAnimator(girlIndex).GetCurrentAnimatorStateInfo(AnimationLayer);

        protected IEnumerable<Bone> GetSupportedBones(int girlIndex) =>
            Enumerable.Concat(new[] { Bone.Auto }, GetFemaleBones(girlIndex).Keys);

        private string GetExactPose(int girlIndex, Bone bone)
            => GetPose(girlIndex) + "." + bone;

        protected bool TryGetPhase(int girlIndex, Bone bone, out float phase)
        {
            string pose = GetExactPose(girlIndex, 0);
            float placeholder = -1f;
            if (!animPhases.TryGetValue(pose, out phase))
            {
                animPhases[pose] = placeholder; // avoid multiple interleaving calls
                HandleCoroutine(ComputeAnimationOffsets(girlIndex));
                return false;
            }
            return animPhases.TryGetValue(GetExactPose(girlIndex, bone), out phase)
                && phase != placeholder;
        }

        protected int GetStrokesPerAnimationCycle(int girlIndex, Bone bone)
        {
            animFreqs.TryGetValue(GetExactPose(girlIndex, bone), out int strokes);
            return strokes == 0 ? 1 : strokes;
        }

        protected virtual float GetStrokeTimeSecs(int girlIndex, Bone bone)
        {
            var info = GetAnimatorStateInfo(girlIndex);
            float strokeTimeSecs = info.length / info.speed
                / GetStrokesPerAnimationCycle(girlIndex, bone);
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
            float startTime = GetAnimatorStateInfo(girlIndex).normalizedTime;
            while (GetAnimatorStateInfo(girlIndex).normalizedTime - 1 < startTime)
            {
                yield return new WaitForEndOfFrame();
                foreach (var entry in femaleBones)
                {
                    var boneF = entry.Value;
                    float distanceSq = (boneM.position - boneF.position).sqrMagnitude;
                    measurements.Add(new Measurement
                    {
                        Bone = entry.Key,
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
            foreach (var bone in femaleBones.Keys)
            {
                animPhases[GetExactPose(girlIndex, bone)] = measurements
                    .Where(entry => entry.Bone == bone)
                    .OrderBy(entry => entry.DistanceSq)
                    .FirstOrDefault()
                    .Time % 1;
                animFreqs[GetExactPose(girlIndex, bone)] = GetFrequency(measurements
                    .Where(entry => entry.Bone == bone)
                    .OrderBy(entry => entry.Time)
                    .Select(entry => entry.DistanceSq));
            }
            var closest = measurements
                .OrderBy(entry => entry.DistanceSq)
                .FirstOrDefault();
            animPhases[GetExactPose(girlIndex, Bone.Auto)] = closest.Time % 1;
            animFreqs[GetExactPose(girlIndex, Bone.Auto)] =
                animFreqs[GetExactPose(girlIndex, closest.Bone)];
            CoreConfig.Logger.LogInfo($"Calibration for pose {pose} completed. " +
                $"{measurements.Count / femaleBones.Count} frames inspected. " +
                $"Closest bone: {closest.Bone}, offset: {closest.Time % 1}, " +
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
            public Bone Bone { get; set; }
            public float Time { get; set; }
            public float DistanceSq { get; set; }
        }
    }
}
