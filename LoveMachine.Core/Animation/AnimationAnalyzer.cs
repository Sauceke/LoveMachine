using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using UnityEngine;

namespace LoveMachine.Core
{
    public abstract class AnimationAnalyzer : CoroutineHandler
    {
        // pose -> result
        private static readonly Dictionary<string, WaveInfo> resultCache =
            new Dictionary<string, WaveInfo>();

        // girl index -> thing that runs calibration
        private static readonly Dictionary<int, CoroutineHandler> containers =
            new Dictionary<int, CoroutineHandler>();

        protected abstract int AnimationLayer { get; }

        protected abstract Animator GetFemaleAnimator(int girlIndex);
        protected abstract Dictionary<Bone, Transform> GetFemaleBones(int girlIndex);
        protected abstract Transform GetMaleBone();
        protected abstract string GetPose(int girlIndex);

        protected virtual IEnumerator WaitAfterPoseChange()
        {
            yield return new WaitForSeconds(0.1f);
        }

        protected AnimatorStateInfo GetAnimatorStateInfo(int girlIndex) =>
            GetFemaleAnimator(girlIndex).GetCurrentAnimatorStateInfo(AnimationLayer);

        protected virtual void GetAnimState(int girlIndex, out float normalizedTime,
            out float length, out float speed)
        {
            var info = GetAnimatorStateInfo(girlIndex);
            normalizedTime = info.normalizedTime;
            length = info.length;
            speed = info.speed;
        }

        protected IEnumerable<Bone> GetSupportedBones(int girlIndex) =>
            Enumerable.Concat(new[] { Bone.Auto }, GetFemaleBones(girlIndex).Keys);

        private string GetExactPose(int girlIndex, Bone bone) =>
            $"{GetPose(girlIndex)}.girl{girlIndex}.{bone}";

        protected bool TryGetWaveInfo(int girlIndex, Bone bone, out WaveInfo result)
        {
            try
            {
                return resultCache.TryGetValue(GetExactPose(girlIndex, bone), out result);
            }
            catch (Exception e)
            {
                CoreConfig.Logger.LogError($"Error while trying to get wave info: {e}");
                result = new WaveInfo();
                return false;
            }
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
                CoreConfig.Logger.LogDebug("New animation playing, starting to analyze.");
                yield return HandleCoroutine(AnalyzeAnimation(girlIndex, updateDictionary),
                    suppressExceptions: true);
            }
        }

        private IEnumerator AnalyzeAnimation(int girlIndex,
            Action<Dictionary<Bone, WaveInfo>> onSuccess)
        {
            var boneM = GetMaleBone();
            var femaleBones = GetFemaleBones(girlIndex);
            string pose = GetExactPose(girlIndex, Bone.Auto);
            var samples = new List<Sample>();
            yield return HandleCoroutine(WaitAfterPoseChange());
            GetAnimState(girlIndex, out float startTime, out _, out _);
            float currentTime = startTime;
            while (currentTime - 1 < startTime)
            {
                yield return new WaitForEndOfFrame();
                GetAnimState(girlIndex, out currentTime, out _, out _);
                foreach (var entry in femaleBones)
                {
                    var boneF = entry.Value;
                    float distance = (boneM.position - boneF.position).magnitude;
                    samples.Add(new Sample
                    {
                        Bone = entry.Key,
                        Time = currentTime,
                        Distance = distance
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
                var bonePath = samples.Where(entry => entry.Bone == bone);
                results[bone] = new WaveInfo
                {
                    Phase = bonePath
                        .OrderBy(entry => entry.Distance)
                        .FirstOrDefault()
                        .Time % 1,
                    Frequency = GetFrequency(bonePath
                        .OrderBy(entry => entry.Time)
                        .Select(entry => entry.Distance)),
                    Crest = bonePath
                        .Select(entry => entry.Distance)
                        .Max(),
                    Trough = bonePath
                        .Select(entry => entry.Distance)
                        .Min()
                };
            }
            // Prefer bones that are close and move a lot. Being close is more important.
            var autoBone = results
                .OrderBy(entry => entry.Value.Trough * entry.Value.Trough
                    / (entry.Value.Crest - entry.Value.Trough))
                .FirstOrDefault()
                .Key;
            results[Bone.Auto] = results[autoBone];
            onSuccess(results);
            CoreConfig.Logger.LogInfo($"Calibration for pose {pose} completed. " +
                $"{samples.Count / femaleBones.Count} frames inspected. " +
                $"Leading bone: {autoBone}, result: {JsonMapper.ToJson(results[Bone.Auto])}.");
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

        internal void ClearCache() => resultCache.Clear();

        private struct Sample
        {
            public Bone Bone { get; set; }
            public float Time { get; set; }
            public float Distance { get; set; }
        }

        protected struct WaveInfo
        {
            public float Phase { get; set; }
            public int Frequency { get; set; }
            public float Crest { get; set; }
            public float Trough { get; set; }
        }
    }
}
