using HarmonyLib;
using LoveMachine.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace LoveMachine.HKR
{
    internal class HolyKnightRiccaGame : GameDescriptor
    {
        private PlayableDirector director;
        private TimelineAsset timeline;
        private Traverse<string> cutName;
        private Dictionary<string, TimelineClip> clipCache;

        public void StartH(MonoBehaviour uiController)
        {
            cutName = Traverse.Create(uiController)
                .Property("actorController")
                .Property<string>("currentCutName");
            StartH();
        }

        public override int AnimationLayer => 0;

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "DEF-clitoris" },
            { Bone.Mouth, "MouseTransform" },
            { Bone.RightHand, "DEF-f_index_01_R" },
            { Bone.LeftHand, "DEF-f_index_01_L" },
            { Bone.RightBreast, "DEF-nipple_R" },
            { Bone.LeftBreast, "DEF-nipple_L" }
        };

        protected override int HeroineCount => 1;

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => true;

        protected override bool IsHSceneInterrupted => false;

        public override Animator GetFemaleAnimator(int girlIndex) =>
            throw new NotImplementedException();

        protected override Transform GetDickBase() =>
            new[] { "DEF-testicle", "ORG-testicle" }
                .Select(GameObject.Find)
                .First(go => go != null)
                .transform;

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            GameObject.Find("ricasso/root");

        protected override string GetPose(int girlIndex) => cutName.Value;

        protected override bool IsIdle(int girlIndex) => false;

        private float lastPartialTime = 0f;
        private int totalTime = 0;

        protected override void GetAnimState(int girlIndex, out float normalizedTime,
            out float length, out float speed)
        {
            string pose = GetPose(0);
            if (!clipCache.TryGetValue(pose, out var clip))
            {
                clip = GetCurrentClip();
                if (clip == null)
                {
                    normalizedTime = 0f;
                    length = 1f;
                    speed = 1f;
                    return;
                }
                clipCache[pose] = clip;
            }
            float partialTime = (float)((director.time - clip.start) / clip.duration);
            if (partialTime < lastPartialTime)
            {
                totalTime += Mathf.CeilToInt(lastPartialTime - partialTime);
            }
            normalizedTime = partialTime + totalTime;
            length = (float)clip.duration;
            speed = 1f;
            lastPartialTime = partialTime;
        }

        private TimelineClip? GetCurrentClip()
        {
            for (int i = 0; i < timeline.outputTrackCount; i++)
            {
                var track = timeline.GetOutputTrack(i);
                var binding = director.GetGenericBinding(track);
                if (binding == null || binding.name != "RicassoKnightActor_atd")
                {
                    continue;
                }
                foreach (var clip in track.clips)
                {
                    if (clip.displayName.StartsWith("SimpleCharacterAnimationClipPlayableAsset")
                        && clip.start <= director.time && director.time <= clip.end)
                    {
                        return clip;
                    }
                }
            }
            return null;
        }

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSeconds(5f);
            director = GameObject.Find("ATDTimeline").GetComponent<PlayableDirector>();
            timeline = director.playableAsset.Cast<TimelineAsset>();
            clipCache = new Dictionary<string, TimelineClip>();
        }
    }
}