using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.COM3D2
{
    internal abstract class Com3d2ButtplugController : ButtplugController
    {
        // Delete some parent path
        private const string SpineF = "Bip01/Bip01 Spine/Bip01 Spine0a/" +
                        "Bip01 Spine1/Bip01 Spine1a";
        private const string PelvisF = "Bip01/Bip01 Pelvis";

        private readonly string[] idlePoseNames = { "taiki", "nade", "shaseigo" };

        // TODO 3some, groups
        protected override int HeroineCount => 1;

        // TOOD animation name numbering is not consistent, need to make some sense out of it
        protected override bool IsHardSex => GetPose(0).Contains('2');

        protected override bool IsHSceneInterrupted => false;

        protected override int AnimationLayer => throw new NotImplementedException();

        protected override Animator GetFemaleAnimator(int girlIndex) =>
            throw new NotImplementedException();

        private float lastPartialTime = 0f;
        private int totalTime = 0;

        protected override void GetAnimState(int girlIndex, out float normalizedTime,
            out float length, out float speed)
        {
            var state = GetActiveState();
            float partialTime = state.normalizedTime;
            // Yes, this is horrible. So is COM3D2's code, so I don't care.
            if (partialTime < lastPartialTime)
            {
                totalTime += Mathf.CeilToInt(lastPartialTime - partialTime);
            }
            normalizedTime = partialTime + totalTime;
            length = state.length;
            speed = state.speed;
            lastPartialTime = partialTime;
            return;
        }

        private AnimationState GetActiveState()
        {
            var animations = FindObjectsOfType<Animation>()
                .Where(animation => animation.name == "_BO_mbody");
            foreach (var animation in animations)
            {
                foreach (AnimationState state in animation)
                {
                    if (animation.IsPlaying(state.name))
                    {
                        return state;
                    }
                }
            }
            return null;
        }

        // This time I will keep the path of maids and man
        protected override Dictionary<Bone, Transform> GetFemaleBones(int girlIndex)
            => new Dictionary<Bone, Transform>
            {
                { Bone.Vagina, FindComponentObject("Maid[", PelvisF + "/_IK_vagina").transform },
                {
                    Bone.RightHand,
                    FindComponentObject("Maid[", SpineF + "/Bip01 R Clavicle/Bip01 R UpperArm/" +
                        "Bip01 R Forearm/Bip01 R Hand/_IK_handR").transform
                },
                {
                    Bone.Mouth,
                    FindComponentObject("Maid[", SpineF + "/Bip01 Neck/Bip01 Head/_SM_face007/MouthUp").transform
                },
                { Bone.LeftBreast, FindComponentObject("Maid[", SpineF + "/Mune_L/_IK_muneL").transform },
                { Bone.RightBreast, FindComponentObject("Maid[", SpineF + "/Mune_R/_IK_muneR").transform },
                {
                    Bone.RightFoot,
                    FindComponentObject("Maid[", PelvisF + "/Bip01 L Thigh/Bip01 L Calf/_IK_calfL").transform
                },
                {
                    Bone.LeftFoot,
                    FindComponentObject("Maid[", PelvisF + "/Bip01 R Thigh/Bip01 R Calf/_IK_calfR").transform
                }
            };

        // Just delete the parent path
        protected override Transform GetMaleBone() =>
            FindComponentObject("Man[", "ManBip/ManBip Pelvis/chinkoCenter/tamabukuro")
                .transform;
        
        /**
         * Find the root character object, the gameobject may not be find by just using GameObject.Find()
         *  pattern - "Maid[" or "Man["
         */
        private List<GameObject> FindCharaObject(string pattern) {
            GameObject characterRootObject = GameObject.Find("__GameMain__/Character/Active/AllOffset");
            List<GameObject> list = new List<GameObject>();
            if (characterRootObject != null)
            {
                int childCount = characterRootObject.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = characterRootObject.transform.GetChild(i);
                    if (child != null && child.gameObject != null && child.gameObject.name.StartsWith(pattern))
                    {
                        GameObject gameObject = child.gameObject;
                        if (gameObject != null)
                        {
                            Transform transform = gameObject.transform.Find("Offset");
                            if (transform != null && transform.childCount > 0)
                            {
                                GameObject childGameObject = transform.GetChild(0).gameObject;
                                if (childGameObject != null && childGameObject.gameObject != null)
                                {
                                    list.Add(childGameObject);
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }
        
        /**
         * Find the gameObject by the component path, the function will try to find the root object everytime
         * may consider optimize the program by only find root chara object once.
         *  pattern - "Maid[" or "Man["
         *  path - the path of component
         */
        private Transform FindComponentObject(string pattern, string path)
        {
            // Find the root character object
            CoreConfig.Logger.LogInfo("Try to find object: " + pattern + " " + path);
            List<GameObject> list = FindCharaObject(pattern);
            Transform trans = null;
            
            // Try to find the component by the complete path
            foreach (GameObject rootObject in list)
            {
                trans = rootObject.transform.Find(path);
                if (trans != null)
                {
                    break;
                }
            }
            
            // If the program can not find the component, it will try to use the name of the component to match every child of the root chara by recursion
            if (trans == null)
            {
                foreach (GameObject rootObject in list)
                {
                    string[] paths = path.Split('/');
                    CoreConfig.Logger.LogInfo("Path is invaild, try to find " + paths[paths.Length - 1]);
                    trans = FindChild(rootObject.transform, paths[paths.Length - 1].Trim());
                    if (trans != null)
                    {
                        CoreConfig.Logger.LogInfo("Find unnamed object: " + GetGameObjectPath(trans.gameObject));
                        break;
                    }
                }
                if (trans == null)
                    CoreConfig.Logger.LogWarning("the object is null");
            }
            return trans;
        }
        
        /**
         * Get the absolute path of the gameobject
         */
        public static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }
        
        /**
         * Find the child with target name by recursion
         *  _t - parent game object
         *  name - name of target child game object
         */
        public static Transform FindChild(Transform _t, string name)
        {
            Transform target = null;
            foreach (Transform t in _t)
            {
                if (t.name.Trim().Equals(name))
                    target = t;
                if (t.childCount > 0)
                    target = FindChild(t, name);
                if (target != null)
                    break;
            }
            return target;
        }

        protected override string GetPose(int girlIndex) => GetActiveState()?.name;

        protected override bool IsIdle(int girlIndex)
        {
            string pose = GetPose(girlIndex);
            return idlePoseNames.Any(pose.Contains);
        }

        protected override IEnumerator UntilReady()
        {
            yield return new WaitForSecondsRealtime(5f);
        }
    }

    internal class Com3d2ButtplugVibeController : Com3d2ButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone) =>
            RunVibratorLoop(girlIndex, bone);
    }

    internal class Com3d2ButtplugStrokerController : Com3d2ButtplugController
    {
        protected override IEnumerator Run(int girlIndex, Bone bone) =>
            RunStrokerLoop(girlIndex, bone);
    }
}
