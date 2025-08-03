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
        private UnityEngine.Object motionCtrl;
        private Il2CppSystem.Reflection.PropertyInfo loopCountProp;
        private Il2CppSystem.Reflection.PropertyInfo playingClipNameProp;
        private Il2CppSystem.Reflection.PropertyInfo playingClipProp;
        private Il2CppSystem.Reflection.PropertyInfo speedProp;

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
                normalizedTime = loopCountProp.GetValue(motionCtrl).Unbox<float>();
                length = playingClipProp.GetValue(motionCtrl).Cast<AnimationClip>().length;
                speed = (float)speedProp.GetValue(motionCtrl).Unbox<double>();
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
                return playingClipNameProp.GetValue(motionCtrl).ToString();
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
            var motionCtrlType = Il2CppType.From(
                Type.GetType("Vrk.Interactive.MotionCtrlSubst, Assembly-CSharp"));
            motionCtrl = FindObjectOfType(motionCtrlType);
            loopCountProp = motionCtrlType.GetProperty("LoopCount");
            playingClipNameProp = motionCtrlType.GetProperty("PlayingClipName");
            playingClipProp = motionCtrlType.GetProperty("PlayingClip");
            speedProp = motionCtrlType.GetProperty("Speed");
        }
    }
}
