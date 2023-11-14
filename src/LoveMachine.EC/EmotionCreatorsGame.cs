using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using HEdit;
using HPlay;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using UnityEngine;

namespace LoveMachine.EC
{
    public class EmotionCreatorsGame : GameAdapter
    {
        private ChaControl[] females;
        private Transform[] penisBases;
        
        protected override MethodInfo[] StartHMethods =>
            new [] { AccessTools.Method(typeof(HPlayScene), nameof(HPlayScene.Start)) };

        protected override MethodInfo[] EndHMethods =>
            new [] { AccessTools.Method(typeof(HPlayScene), nameof(HPlayScene.OnDestroy)) };
        
        protected override Dictionary<Bone, string> FemaleBoneNames => new Dictionary<Bone, string>
        {
            { Bone.LeftBreast, "k_f_munenipL_00" },
            { Bone.RightBreast, "k_f_munenipR_00" },
            { Bone.Vagina, "cf_n_pee" },
            { Bone.Anus, "k_f_ana_00" },
            { Bone.LeftButt, "k_f_siriL_00" },
            { Bone.RightButt, "k_f_siriR_00" },
            { Bone.Mouth, "cf_J_MouthCavity" },
            { Bone.LeftHand, "cf_j_index04_L" },
            { Bone.RightHand, "cf_j_index04_R" },
            { Bone.LeftFoot, "k_f_toeL_00" },
            { Bone.RightFoot, "k_f_toeR_00" },
        };

        protected override Transform PenisBase => throw new NotImplementedException();
        protected override Transform[] PenisBases => penisBases;
        protected override int AnimationLayer => 0;
        protected override int HeroineCount => females.Length;
        protected override int MaxHeroineCount => 5;
        protected override bool IsHardSex => false;

        protected override Animator GetFemaleAnimator(int girlIndex) => females[girlIndex].animBody;

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            females[girlIndex].objBodyBone;

        protected override string GetPose(int girlIndex) =>
            HPlayData.Instance.groupInfos.First().nowMotion.ToString();

        protected override bool IsIdle(int girlIndex) =>
            HPlayData.Instance.groupInfos.First().nowMotion == 0;
        
        protected override IEnumerator UntilReady()
        {
            while (HPlayData.Instance?.basePart?.kind != 0)
            {
                yield return new WaitForSeconds(5f);
            }
            var hPart = (HPart)HPlayData.Instance.basePart;
            var charas = hPart.groups
                .SelectMany(group => group.infoCharas)
                .Select(info => HEditData.Instance.charas[info.useCharaID]);
            females = charas.Where(chara => chara.sex == 1).ToArray();
            penisBases = charas.Where(chara => chara.sex == 0)
                .SelectMany(cha => FindDeepChildrenByName(cha.objBodyBone, "k_f_tamaL_00"))
                .ToArray();
        }
    }
}
