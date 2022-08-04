using System.Collections;
using System.Collections.Generic;
using LoveMachine.Core;
using UnityEngine;

namespace LoveMachine.KK
{
    internal abstract class KoikatsuAibuController : ButtplugController
    {
        private static readonly List<Bone> aibuBones = new List<Bone>
        {
            Bone.LeftBreast, Bone.RightBreast, Bone.Vagina, Bone.Anus, Bone.LeftButt, Bone.RightButt
        };

        private KoikatsuGame kk;

        protected override void Start()
        {
            base.Start();
            kk = gameObject.GetComponent<KoikatsuGame>();
        }

        protected abstract void HandleFondle(Device device, float y, float timeSecs);

        protected override IEnumerator Run(Device device)
        {
            float updateTimeSecs = 1f / device.Settings.UpdatesHz;
            float previousY = 0f;
            while (true)
            {
                yield return new WaitForSecondsRealtime(updateTimeSecs);
                Bone bone = device.Settings.Bone;
                if (!aibuBones.Contains(bone))
                {
                    continue;
                }
                float y = kk.Flags.xy[aibuBones.IndexOf(bone)].y;
                if (previousY != y)
                {
                    HandleFondle(device, y, timeSecs: updateTimeSecs);
                    previousY = y;
                }
            }
        }
    }

    internal sealed class KoikatsuAibuStrokerController : KoikatsuAibuController
    {
        protected override bool IsDeviceSupported(Device device) => device.IsStroker;

        protected override void HandleFondle(Device device, float y, float timeSecs) =>
            client.LinearCmd(device, y, timeSecs);
    }

    internal sealed class KoikatsuAibuVibratorController : KoikatsuAibuController
    {
        protected override bool IsDeviceSupported(Device device) => device.IsVibrator;

        protected override void HandleFondle(Device device, float y, float timeSecs) =>
            client.VibrateCmd(device, y);
    }
}
