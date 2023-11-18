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
        private Chara[] females;
        private Transform[] penisBases;
        
        protected override MethodInfo[] StartHMethods =>
            new [] { AccessTools.Method(typeof(HPlayScene), nameof(HPlayScene.InitPart)) };

        protected override MethodInfo[] EndHMethods =>
            new [] { AccessTools.Method(typeof(HPlayScene), nameof(HPlayScene.SceneEndProc)) };
        
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

        protected override Animator GetFemaleAnimator(int girlIndex) =>
            females[girlIndex].ChaControl.animBody;

        protected override GameObject GetFemaleRoot(int girlIndex) =>
            females[girlIndex].ChaControl.objBodyBone;

        protected override string GetPose(int girlIndex) => GetMotion(girlIndex).ToString();

        protected override bool IsIdle(int girlIndex) => GetMotion(girlIndex) != 1;

        protected override bool IsOrgasming(int girlIndex) =>
            GetMotion(girlIndex) > 1 && GetMotion(girlIndex) < 6;

        protected override IEnumerator UntilReady()
        {
            while (HPlayData.Instance?.basePart?.kind != 0)
            {
                yield return new WaitForSeconds(5f);
            }
            var hPart = (HPart)HPlayData.Instance.basePart;
            var charas = hPart.groups
                .Select(group => group.infoCharas)
                .Select(infos => infos.Select(info => HEditData.Instance.charas[info.useCharaID]))
                .Select((lst, i) => lst.Select(cha => new Chara { GroupID = i, ChaControl = cha }))
                .SelectMany(lst => lst);
            females = charas.Where(chara => chara.ChaControl.sex == 1).ToArray();
            penisBases = charas
                .Select(chara => chara.ChaControl)
                .Where(cha => cha.sex == 0)
                .SelectMany(cha => FindDeepChildrenByPath(cha.objBodyBone, "k_f_tamaL_00"))
                .ToArray();
        }

        private int GetMotion(int girlIndex) =>
            HPlayData.Instance.groupInfos[females[girlIndex].GroupID].nowMotion;
        
        private struct Chara
        {
            public int GroupID { get; set; }
            public ChaControl ChaControl { get; set; }
        }
    }
}
