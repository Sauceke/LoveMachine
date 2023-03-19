using Il2CppInterop.Runtime.Attributes;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoveMachine.Core
{
    public class AnimationAnalyzer : CoroutineHandler
    {
        // com3d2 has animations with more than 10 strokes, but those aren't
        // evenly spaced, so can't do much about them atm
        private const int MaxFrequency = 10;

        // pose -> result
        private readonly Dictionary<string, WaveInfo> resultCache =
            new Dictionary<string, WaveInfo>();

        private GameDescriptor game;

        private void Start()
        {
            game = GetComponent<GameDescriptor>();
            game.OnHStarted += (s, a) => StartAnalyze();
            game.OnHEnded += (s, a) => StopAnalyze();
        }

        private string GetExactPose(int girlIndex, Bone bone) =>
            $"{game.GetPose(girlIndex)}.girl{girlIndex}.{bone}";

        [HideFromIl2Cpp]
        public virtual bool TryGetWaveInfo(int girlIndex, Bone bone, out WaveInfo result)
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

        private void StartAnalyze()
        {
            StopAllCoroutines();
            Enumerable.Range(0, game.HeroineCount).ToList()
                .ForEach(girlIndex => HandleCoroutine(RunAnalysisLoop(girlIndex)));
        }

        private void StopAnalyze()
        {
            StopAllCoroutines();
            resultCache.Clear();
        }

        private IEnumerator RunAnalysisLoop(int girlIndex)
        {
            while (true)
            {
                if (TryGetWaveInfo(girlIndex, Bone.Auto, out var _))
                {
                    yield return new WaitForSecondsRealtime(0.1f);
                    continue;
                }
                CoreConfig.Logger.LogDebug("New animation playing, starting to analyze.");
                yield return HandleCoroutine(AnalyzeAnimation(girlIndex), suppressExceptions: true);
            }
        }

        private IEnumerator AnalyzeAnimation(int girlIndex)
        {
            var penisBases = game.PenisBases;
            var femaleBones = game.GetFemaleBones(girlIndex);
            string pose = GetExactPose(girlIndex, Bone.Auto);
            yield return HandleCoroutine(game.WaitAfterPoseChange());
            var samples = new List<Sample>();
            game.GetAnimState(girlIndex, out float startTime, out _, out _);
            float currentTime = startTime;
            while (currentTime - 1f < startTime)
            {
                yield return new WaitForEndOfFrame();
                game.GetAnimState(girlIndex, out currentTime, out _, out _);
                var newSamples = femaleBones
                    .SelectMany(entry => penisBases, (entry, penisBase) => new Sample
                    {
                        Bone = entry.Key,
                        PenisBase = penisBase,
                        Time = currentTime,
                        RelativePos = penisBase.position - entry.Value.position
                    });
                samples.AddRange(newSamples);
                if (pose != GetExactPose(girlIndex, Bone.Auto) || currentTime < startTime)
                {
                    CoreConfig.Logger.LogWarning($"Pose {pose} interrupted; canceling analysis.");
                    yield break;
                }
            }
            var results = femaleBones.Keys
                .ToDictionary(bone => bone,
                    bone => GetPreferredWaveInfo(samples.Where(entry => entry.Bone == bone)));
            var autoBone = results
                .OrderBy(result => result.Value.Preference)
                .FirstOrDefault()
                .Key;
            results[Bone.Auto] = results[autoBone];
            results.ToList()
                .ForEach(kvp => resultCache[GetExactPose(girlIndex, kvp.Key)] = kvp.Value);
            CoreConfig.Logger.LogInfo($"Calibration for pose {pose} completed. " +
                $"{samples.Count / femaleBones.Count} frames inspected. " +
                $"Leading bone: {autoBone}, result: {JsonMapper.ToJson(results[Bone.Auto])}.");
        }

        private static WaveInfo GetPreferredWaveInfo(IEnumerable<Sample> samples) => samples
            .GroupBy(sample => sample.PenisBase)
            .Select(GetWaveInfo)
            .OrderBy(waveInfo => waveInfo.Preference)
            .First();

        private static WaveInfo GetWaveInfo(IEnumerable<Sample> samples)
        {
            // probably safe to assume the farthest point from the origin is an extremity
            var crest = samples
                .OrderBy(sample => -sample.RelativePos.magnitude)
                .First();
            var trough = samples
                .OrderBy(sample => -(sample.RelativePos - crest.RelativePos).magnitude)
                .First();
            var axis = crest.RelativePos - trough.RelativePos;
            float GetDistance(Vector3 v) =>
                Vector3.Project(v - trough.RelativePos, axis).magnitude;
            var distances = samples.Select(sample => GetDistance(sample.RelativePos));
            return new WaveInfo
            {
                Phase = trough.Time % 1f,
                Frequency = GetFrequency(distances),
                Amplitude = axis.magnitude,
                // Prefer bones that are close and move a lot. Being close is more important.
                Preference = axis.magnitude == 0
                    ? float.PositiveInfinity
                    : Mathf.Pow(trough.RelativePos.magnitude, 3f) / axis.magnitude
            };
        }

        private static int GetFrequency(IEnumerable<float> samples)
        {
            // Catch flatlines.
            const float epsilon = 0.000001f;
            if (samples.Max() - samples.Min() <= epsilon)
            {
                return 1;
            }
            // Cap to Nyquist frequency.
            // Why not collect samples until we have enough of them?
            // 1. Because it would increase downtime for the device.
            // 2. Because we might end up collecting samples from the same
            //    spots over and over again, and thus never have enough of them to
            //    meaningfully test for higher frequencies.
            var maxFreq = Mathf.Min(MaxFrequency, samples.Count() / 2);
            // Get frequency using Fourier series.
            var dfsMagnitudes = new float[maxFreq];
            for (int k = 1; k <= maxFreq; k++)
            {
                float freq = 2f * Mathf.PI / samples.Count() * k;
                float re = samples.Select((amp, index) => amp * Mathf.Cos(freq * index)).Sum();
                float im = samples.Select((amp, index) => amp * Mathf.Sin(freq * index)).Sum();
                dfsMagnitudes[k - 1] = re * re + im * im;
            }
            return Array.IndexOf(dfsMagnitudes, dfsMagnitudes.Max()) + 1;
        }

        private struct Sample
        {
            public Bone Bone { get; set; }
            public Transform PenisBase { get; set; }
            public float Time { get; set; }
            public Vector3 RelativePos { get; set; }
        }
    }
}