using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoveMachine.Core
{
    public abstract class AnimationAnalyzer : CoroutineHandler
    {
        // pose -> result
        private static readonly Dictionary<string, WaveInfo> resultCache
            = new Dictionary<string, WaveInfo>();

        // girl index -> thing that runs calibration
        private static readonly Dictionary<int, CoroutineHandler> containers
            = new Dictionary<int, CoroutineHandler>();

        protected abstract int AnimationLayer { get; }
        protected abstract Animator GetFemaleAnimator(int girlIndex);
        protected abstract Dictionary<Bone, Transform> GetFemaleBones(int girlIndex);
        protected abstract Transform GetMaleBone();
        protected abstract string GetPose(int girlIndex);

        protected AnimatorStateInfo GetAnimatorStateInfo(int girlIndex)
            => GetFemaleAnimator(girlIndex).GetCurrentAnimatorStateInfo(AnimationLayer);

        protected IEnumerable<Bone> GetSupportedBones(int girlIndex)
            => Enumerable.Concat(new[] { Bone.Auto }, GetFemaleBones(girlIndex).Keys);

        private string GetExactPose(int girlIndex, Bone bone)
            => $"{GetPose(girlIndex)}.girl{girlIndex}.{bone}";

        protected bool TryGetWaveInfo(int girlIndex, Bone bone, out WaveInfo result)
            => resultCache.TryGetValue(GetExactPose(girlIndex, bone), out result);

        protected virtual float GetStrokeTimeSecs(int girlIndex, Bone bone)
        {
            var info = GetAnimatorStateInfo(girlIndex);
            int freq = TryGetWaveInfo(girlIndex, bone, out var result) ? result.Frequency : 1;
            float strokeTimeSecs = info.length / info.speed / freq;
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

        protected void StartAnalyze(int girlIndex)
        {
            if (!containers.TryGetValue(girlIndex, out var container))
            {
                containers[girlIndex] = container = gameObject.AddComponent<CoroutineHandler>();
            }
            // never run the same coroutine twice, even if the last one wasn't cleaned up
            container.StopAllCoroutines();
            container.HandleCoroutine(RunAnalysisLoop(girlIndex));
        }

        protected void StopAnalyze(int girlIndex)
        {
            if (!containers.TryGetValue(girlIndex, out var container))
            {
                return;
            }
            container.StopAllCoroutines();
        }

        private IEnumerator RunAnalysisLoop(int girlIndex)
        {
            void updateDictionary(Dictionary<Bone, WaveInfo> results) => results.ToList()
                .ForEach(kvp => resultCache[GetExactPose(girlIndex, kvp.Key)] = kvp.Value);
            while (true)
            {
                if (TryGetWaveInfo(girlIndex, Bone.Auto, out var _))
                {
                    yield return new WaitForSecondsRealtime(0.1f);
                    continue;
                }
                yield return HandleCoroutine(AnalyzeAnimation(girlIndex, updateDictionary));
            }
        }

        private IEnumerator AnalyzeAnimation(int girlIndex,
            Action<Dictionary<Bone, WaveInfo>> onSuccess)
        {
            var boneM = GetMaleBone();
            var femaleBones = GetFemaleBones(girlIndex);
            string pose = GetExactPose(girlIndex, Bone.Auto);
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
            if (pose != GetExactPose(girlIndex, Bone.Auto))
            {
                CoreConfig.Logger.LogWarning($"Pose {pose} interrupted; canceling calibration.");
                yield break;
            }
            var results = new Dictionary<Bone, WaveInfo>();
            foreach (var bone in femaleBones.Keys)
            {
                results[bone] = new WaveInfo
                {
                    Phase = measurements
                        .Where(entry => entry.Bone == bone)
                        .OrderBy(entry => entry.DistanceSq)
                        .FirstOrDefault()
                        .Time % 1,
                    Frequency = GetFrequency(measurements
                        .Where(entry => entry.Bone == bone)
                        .OrderBy(entry => entry.Time)
                        .Select(entry => entry.DistanceSq))
                };
            }
            var closest = measurements
                .OrderBy(entry => entry.DistanceSq)
                .FirstOrDefault();
            results[Bone.Auto] = new WaveInfo
            {
                Phase = results[closest.Bone].Phase,
                Frequency = results[closest.Bone].Frequency
            };
            onSuccess(results);
            CoreConfig.Logger.LogInfo($"Calibration for pose {pose} completed. " +
                $"{measurements.Count / femaleBones.Count} frames inspected. " +
                $"Closest bone: {closest.Bone}, offset: {results[Bone.Auto].Phase}, " +
                $"frequency: {results[Bone.Auto].Frequency}. ");
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

        internal void ClearCache()
        {
            resultCache.Clear();
        }

        private struct Measurement
        {
            public Bone Bone { get; set; }
            public float Time { get; set; }
            public float DistanceSq { get; set; }
        }

        public struct WaveInfo
        {
            public float Phase { get; set; }
            public int Frequency { get; set; }
            public float Crest { get; set; }
            public float Trough { get; set; }
        }
    }
}
