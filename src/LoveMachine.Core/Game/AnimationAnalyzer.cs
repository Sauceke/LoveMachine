using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using LoveMachine.Core.Common;
using LoveMachine.Core.Config;
using LoveMachine.Core.NonPortable;
using UnityEngine;

namespace LoveMachine.Core.Game
{
    internal class AnimationAnalyzer : CoroutineHandler
    {
        // pose -> result
        private readonly Dictionary<TrackingKey, Result> resultCache =
            new Dictionary<TrackingKey, Result>();

        private GameAdapter game;

        private void Start()
        {
            game = GetComponent<GameAdapter>();
            game.OnHStarted += (s, a) => StartAnalyze();
            game.OnHEnded += (s, a) => StopAnalyze();
        }

        [HideFromIl2Cpp]
        public bool TryGetCurrentStrokeInfo(TrackingKey trackingKey, float normalizedTime,
            out StrokeInfo strokeInfo)
        {
            if (!TryGetResult(trackingKey, out var result))
            {
                strokeInfo = default;
                return false;
            }
            var delimiters = result.StrokeDelimiters;
            float animTimeSecs = game.GetAnimationTimeSecs(trackingKey.GirlIndex);
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
        private bool TryGetResult(TrackingKey trackingKey, out Result result)
        {
            try
            {
                var success = resultCache.TryGetValue(trackingKey, out result);
                result = success ? result : new Result();
                return success;
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
            while (true)
            {
                var testKey = new TrackingKey
                {
                    GirlIndex = girlIndex,
                    Bone = Bone.Auto,
                    Pose = game.GetPose(girlIndex),
                    POV = POV.Balanced,
                    Axis = Axis.Longest,
                    MovementType = MovementType.Linear
                };
                if (TryGetResult(testKey, out _))
                {
                    yield return new WaitForSecondsRealtime(0.1f);
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
            string pose = game.GetPose(girlIndex);
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
                        MalePos = penisBase.position,
                        FemalePos = entry.Value.position,
                        MaleRot = penisBase.rotation,
                        FemaleRot = entry.Value.rotation
                    });
                samples.AddRange(newSamples);
                if (pose != game.GetPose(girlIndex) || currentTime < startTime)
                {
                    Logger.LogWarning($"Pose {pose} interrupted; canceling analysis.");
                    yield break;
                }
            }
            var allKeys = GenerateTrackingKeys(girlIndex, pose).ToList();
            var results = samples
                .GroupBy(sample => sample.PenisBase)
                .Select(group =>
                    allKeys.ToDictionary(key => key, key => EvaluateSamples(group, key)))
                .ToList();
            var preferredResults = allKeys.ToDictionary(
                key => key,
                key => results.OrderBy(dict => dict[key].Preference).First()[key]);
            var groupedKeys = allKeys.GroupBy(key =>
                new { key.GirlIndex, key.POV, key.Pose, key.Axis, key.MovementType });
            foreach (var group in groupedKeys)
            {
                var bestKey = group.OrderBy(key => preferredResults[key].Preference).First();
                var autoKey = bestKey;
                autoKey.Bone = Bone.Auto;
                preferredResults[autoKey] = preferredResults[bestKey];
            }
            foreach (var kvp in preferredResults)
            {
                resultCache[kvp.Key] = kvp.Value;
            }
            Logger.LogInfo($"Calibration for pose {pose} completed. " +
                $"{samples.Count / femaleBones.Count} frames inspected.");
        }

        private IEnumerable<TrackingKey> GenerateTrackingKeys(int girlIndex, string pose) =>
            from bone in game.FemaleBoneNames.Keys
            from pov in Enum.GetValues(typeof(POV)).Cast<POV>()
            from axis in Enum.GetValues(typeof(Axis)).Cast<Axis>()
            from movementType in Enum.GetValues(typeof(MovementType)).Cast<MovementType>()
            select new TrackingKey
            {
                GirlIndex = girlIndex,
                Bone = bone,
                Pose = pose,
                POV = pov,
                Axis = axis,
                MovementType = movementType
            };

        private Result EvaluateSamples(IEnumerable<Sample> samples,
            TrackingKey trackingKey)
        {
            samples = samples.Where(sample => sample.Bone == trackingKey.Bone).ToList();
            var femaleCenter = samples
                .Select(sample => sample.FemalePos)
                .Aggregate(Vector3.zero, (acc, pos) => acc + pos / samples.Count());
            var maleFarthest = samples
                .OrderBy(sample => -(sample.MalePos - femaleCenter).sqrMagnitude)
                .First()
                .MalePos;
            var femaleFarthest = samples
                .OrderBy(sample => -(sample.FemalePos - maleFarthest).sqrMagnitude)
                .First()
                .MalePos;
            Vector3 GetRelativePos(Sample sample) =>
                GetRelativePosition(sample, trackingKey.POV, maleFarthest, femaleFarthest);
            var relativePositions = samples.Select(sample => GetRelativePos(sample)).ToList();
            var crest = relativePositions.OrderBy(pos => -pos.magnitude).First();
            var trough = relativePositions.OrderBy(pos => -(pos - crest).magnitude).First();
            var longestAxis = crest - trough;
            Vector3 GetAxis(Sample sample) => this.GetAxis(sample, trackingKey.Axis, longestAxis);
            float GetDistance(Sample sample) =>
                Vector3.Project(GetRelativePos(sample) - trough, GetAxis(sample)).magnitude;
            float GetTwist(Sample sample) =>
                RotationToTwist(GetRelativeRotation(sample, trackingKey.POV), GetAxis(sample));
            var nodes = samples.Select(sample => new Node
            {
                Time = sample.Time,
                Position = trackingKey.MovementType == MovementType.Linear
                    ? GetDistance(sample)
                    : GetTwist(sample)
            }).ToList();
            if (trackingKey.MovementType == MovementType.Rotation)
            {
                nodes = NormalizeAngles(nodes).ToList();
            }
            float amplitude = nodes.Max(node => node.Position) - nodes.Min(node => node.Position);
            return new Result
            {
                StrokeDelimiters = GetStrokeDelimiters(nodes, amplitude * game.MinStrokeLength),
                Amplitude = amplitude,
                // Prefer bones that are close and move a lot. Being close is more important.
                Preference = amplitude == 0
                    ? float.PositiveInfinity
                    : Mathf.Pow(trough.magnitude, 3f) / amplitude
            };
        }

        private Vector3 GetRelativePosition(Sample sample, POV pov, Vector3 male, Vector3 female)
        {
            switch(pov)
            {
                case POV.Balanced:
                    return sample.MalePos - sample.FemalePos;

                case POV.Male:
                    return male - sample.FemalePos;

                case POV.Female:
                    return sample.MalePos - female;

                default:
                    throw new Exception("unreachable");
            }
        }

        public static float RotationToTwist(Quaternion rotation, Vector3 axis)
        {
            (rotation * Quaternion.FromToRotation(rotation * axis, axis))
                .ToAngleAxis(out float angle, out _);
            return angle;
        }

        private Quaternion GetRelativeRotation(Sample sample, POV pov)
        {
            switch (pov) {
                case POV.Balanced:
                    return sample.MaleRot * Quaternion.Inverse(sample.FemaleRot);

                case POV.Male:
                    return sample.FemaleRot;

                case POV.Female:
                    return sample.MaleRot;

                default:
                    throw new Exception("unreachable");
            }
        }

        private IEnumerable<Node> NormalizeAngles(IEnumerable<Node> nodes)
        {
            var normalized = new List<Node> { nodes.First() };
            foreach (var node in nodes.Skip(1))
            {
                float lastAngle = normalized.Last().Position;
                float angle = node.Position - lastAngle;
                angle = (angle + 360f + 180f) % 360f - 180f;
                normalized.Add(new Node
                {
                    Time = node.Time,
                    Position = lastAngle + angle
                });
            }
            return normalized;
        }

        private Vector3 GetAxis(Sample sample, Axis axis, Vector3 longest)
        {
            switch(axis)
            {
                case Axis.Longest:
                    return longest;

                case Axis.X:
                    return sample.MaleRot * Vector3.right;

                case Axis.Y:
                    return sample.MaleRot * Vector3.up;

                case Axis.Z:
                    return sample.MaleRot * Vector3.forward;

                default:
                    throw new Exception("unreachable");
            }
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
            public Vector3 MalePos { get; set; }
            public Vector3 FemalePos { get; set; }
            public Quaternion MaleRot { get; set; }
            public Quaternion FemaleRot { get; set; }
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