using System.Collections;
using System.Collections.Generic;
using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Common;
using LoveMachine.Core.Controller.Addons;
using UnityEngine;

namespace LoveMachine.KK
{
    internal class FondleGimmick : Gimmick
    {
        private static readonly List<Bone> fondleBones = new List<Bone>
        {
            Bone.LeftBreast, Bone.RightBreast, Bone.Vagina, Bone.Anus, Bone.LeftButt, Bone.RightButt
        };

        protected override IEnumerator Run(DeviceFeature feature)
        {
            var kk = gameObject.GetComponent<KoikatsuGame>();
            float updateTimeSecs = 1f / feature.Device.Settings.UpdatesHz;
            float previousY = 0f;
            while (true)
            {
                yield return new WaitForSecondsRealtime(updateTimeSecs);
                var bone = feature.Settings.Bone;
                if (!fondleBones.Contains(bone))
                {
                    continue;
                }
                float y = kk.Flags.xy[fondleBones.IndexOf(bone)].y;
                if (previousY != y)
                {
                    SetLevel(feature, y, updateTimeSecs);
                    previousY = y;
                }
            }
        }
    }
}