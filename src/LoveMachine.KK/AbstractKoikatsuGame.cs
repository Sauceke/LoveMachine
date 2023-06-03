using LoveMachine.Core;
using System.Collections.Generic;

namespace LoveMachine.KK
{
    internal abstract class AbstractKoikatsuGame : GameAdapter
    {
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
    }
}