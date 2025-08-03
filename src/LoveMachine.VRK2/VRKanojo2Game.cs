using HarmonyLib;
using Il2CppInterop.Runtime;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace LoveMachine.VRK2
{
    public class VRKanojo2Game : GameAdapter
    {
        private Component uac;
        private Il2CppSystem.Reflection.PropertyInfo loopCount;
        private Il2CppSystem.Reflection.PropertyInfo playingClip;
        private Il2CppSystem.Reflection.PropertyInfo nowClip;
        private Il2CppSystem.Reflection.PropertyInfo speed;

        protected override MethodInfo[] StartHMethods => new MethodInfo[]
        {
            AccessTools.Method("Vrk.Process.HPartProcess, Assembly-CSharp:Vrk_Process_IProcess_Init")
        };

        protected override MethodInfo[] EndHMethods => new MethodInfo[]
        {
            AccessTools.Method("Vrk.Process.HPartProcess, Assembly-CSharp:Vrk_Process_IProcess_InterruptProcess")
        };

        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.Vagina, "Touch_Crotch" },
            { Bone.Mouth, "KissDetector" },
            { Bone.RightHand, "R_HandIndex5" }
        };

        protected override float MinStrokeLength => 0.25f;

        protected override Transform PenisBase =>
            GameObject.Find("Cha_Player01/Models/Cha_Player01_FP/Reference/Hips/R_Crotch1_null/R_Crotch1").transform;

        protected override int AnimationLayer => throw new NotImplementedException();

        protected override int HeroineCount => 1;

        protected override int MaxHeroineCount => 1;

        protected override bool IsHardSex => false;

        protected override Animator GetFemaleAnimator(int girlIndex) => throw new NotImplementedException();

        protected override void GetAnimState(int girlIndex, out float normalizedTime, out float length, out float speed)
        {
            try
            {
                normalizedTime = loopCount.GetValue(uac).Unbox<float>();
                length = nowClip.GetValue(uac).Cast<AnimationClip>().length;
                speed = (float)this.speed.GetValue(uac).Unbox<double>();
            }
            catch (Exception e)
            {
                Logger.LogDebug(e);
                normalizedTime = 0f;
                length = 1f;
                speed = 1f;
            }
        }

        protected override GameObject GetFemaleRoot(int girlIndex) => GameObject.Find("Cha01_Sakura01");

        protected override string GetPose(int girlIndex)
        {
            try
            {
                return playingClip.GetValue(uac).ToString();
            }
            catch (Exception e)
            {
                Logger.LogDebug(e);
                return "unknown";
            }
        }

        protected override IEnumerator WaitAfterPoseChange()
        {
            yield return new WaitForSeconds(1f);
        }

        protected override bool IsIdle(int girlIndex) => false;

        protected override IEnumerator UntilReady(object instance)
        {
            yield return new WaitForSecondsRealtime(10f);
            var chara = GameObject.Find(
                "Cha01_Sakura01/Models/Cha_Sakura_BodyTop/Cha_Sakura_BodyTop/Reference");
            var animControllerType = Il2CppType.From(
                Type.GetType("Vrk.Interactive.UniversalAnimationController, Assembly-CSharp"));
            uac = chara.GetComponent(animControllerType);
            loopCount = animControllerType.GetProperty("LoopCount");
            playingClip = animControllerType.GetProperty("PlayingClip");
            nowClip = animControllerType.GetProperty("NowClip");
            speed = animControllerType.GetProperty("Speed");
        }
    }
}
