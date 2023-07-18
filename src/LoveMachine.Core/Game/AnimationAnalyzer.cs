using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using LitJson;
using LoveMachine.Core.PlatformSpecific;
using LoveMachine.Core.Common;
using UnityEngine;

namespace LoveMachine.Core.Game
{
    internal class AnimationAnalyzer : CoroutineHandler
    {
        // pose -> result
        private readonly Dictionary<string, Result> resultCache =
            new Dictionary<string, Result>();

        private GameAdapter game;

        private void Start()
        {
            game = GetComponent<GameAdapter>();
            game.OnHStarted += (s, a) => StartAnalyze();
            game.OnHEnded += (s, a) => StopAnalyze();
        }

        private string GetExactPose(int girlIndex, Bone bone) =>
            $"{game.GetPose(girlIndex)}.girl{girlIndex}.{bone}";

        [HideFromIl2Cpp]
        public bool TryGetCurrentStrokeInfo(int girlIndex, Bone bone, float normalizedTime,
            out StrokeInfo strokeInfo)
        {
            if (!TryGetResult(girlIndex, bone, out var result))
            {
                strokeInfo = default;
                return false;
            }
            var delimiters = result.StrokeDelimiters;
            float animTimeSecs = game.GetAnimationTimeSecs(girlIndex);
            int delimIndex = Enumerable.Range(0, delimiters.Length)
                .Where(i => delimiters[i] <= normalizedTime % 1f)
                .DefaultIfEmpty(delimiters.Length - 1)
                .Last();
            float start = delimiters[delimIndex];
            float end = delimIndex == delimiters.Length - 1
                ? delimiters[0] + 1f
                : delimiters[delimIndex + 1];
            if (normalizedTime % 1f < start)
            {
                start -= 1f;
                end -= 1f;
            }
            float normalizedStrokeDuration = end - start;
            strokeInfo = new StrokeInfo
            {
                Amplitude = result.Amplitude,
                DurationSecs = animTimeSecs * normalizedStrokeDuration,
                Completion = Mathf.InverseLerp(start, end, normalizedTime % 1f)
            };
            return true;
        }
        
        [HideFromIl2Cpp]
        private bool TryGetResult(int girlIndex, Bone bone, out Result result)
        {
            try
            {
                return resultCache.TryGetValue(GetExactPose(girlIndex, bone), out result);
            }
            catch (Exception e)
            {
                Logger.LogError($"Error while trying to get wave info: {e}");
                result = new Result();
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
            var rest = new WaitForSecondsRealtime(0.1f);
            while (true)
            {
                if (TryGetResult(girlIndex, Bone.Auto, out _))
                {
                    yield return rest;
                    continue;
                }
                Logger.LogDebug("New animation playing, starting to analyze.");
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
            var waitForEndOfFrame = new WaitForEndOfFrame();
            float currentTime = startTime;
            while (currentTime - 1f < startTime)
            {
                yield return waitForEndOfFrame;
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
                    Logger.LogWarning($"Pose {pose} interrupted; canceling analysis.");
                    yield break;
                }
            }
            var results = femaleBones.Keys
                .ToDictionary(bone => bone,
                    bone => GetPreferredResult(samples.Where(entry => entry.Bone == bone)));
            var autoBone = results
                .OrderBy(result => result.Value.Preference)
                .FirstOrDefault()
                .Key;
            results[Bone.Auto] = results[autoBone];
            results.ToList()
                .ForEach(kvp => resultCache[GetExactPose(girlIndex, kvp.Key)] = kvp.Value);
            Logger.LogInfo($"Calibration for pose {pose} completed. " +
                $"{samples.Count / femaleBones.Count} frames inspected. " +
                $"Leading bone: {autoBone}, result: {JsonMapper.ToJson(results[Bone.Auto])}.");
        }

        private Result GetPreferredResult(IEnumerable<Sample> samples) => samples
            .GroupBy(sample => sample.PenisBase)
            .Select(EvaluateSamples)
            .OrderBy(result => result.Preference)
            .First();

        private Result EvaluateSamples(IEnumerable<Sample> samples)
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
            float amplitude = samples.Max(sample => GetDistance(sample.RelativePos));
            var nodes = samples.Select(sample => new Node
            {
                Time = sample.Time,
                Position = Mathf.InverseLerp(0f, amplitude, GetDistance(sample.RelativePos))
            });
            return new Result
            {
                StrokeDelimiters = GetStrokeDelimiters(nodes, tolerance: game.MinStrokeLength),
                Amplitude = axis.magnitude,
                // Prefer bones that are close and move a lot. Being close is more important.
                Preference = axis.magnitude == 0
                    ? float.PositiveInfinity
                    : Mathf.Pow(trough.RelativePos.magnitude, 3f) / axis.magnitude
            };
        }

        private static float[] GetStrokeDelimiters(IEnumerable<Node> nodes, float tolerance)
        {
            var edge = nodes.OrderBy(node => node.Position).First();
            int index = nodes.ToList().IndexOf(edge);
            nodes = nodes.Skip(index).Concat(nodes.Take(index));
            int direction = 1;
            var edges = new List<Node>();
            foreach (var node in nodes)
            {
                float delta = edge.Position - node.Position;
                edge = Math.Sign(delta) == direction ? node : edge;
                if (Mathf.Abs(delta) > tolerance)
                {
                    edges.Add(edge);
                    edge = node;
                    direction *= -1;
                }
            }
            return edges.Where((node, i) => i % 2 == 0)
                .Select(node => node.Time % 1f)
                .OrderBy(time => time)
                .ToArray();
        }
        
        private struct Sample
        {
            public Bone Bone { get; set; }
            public Transform PenisBase { get; set; }
            public float Time { get; set; }
            public Vector3 RelativePos { get; set; }
        }
        
        private struct Node
        {
            public float Time { get; set; }
            public float Position { get; set; }
        }

        private struct Result
        {
            public float[] StrokeDelimiters { get; set; }
            public float Amplitude { get; set; }
            public float Preference { get; set; } // smaller is better
        }
    }
}