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

        private KoikatsuGame game;

        private void Start() => game = gameObject.GetComponent<KoikatsuGame>();

        protected abstract void HandleFondle(float y, int girlIndex, Bone bone, float timeSecs);

        protected override IEnumerator Run(int girlIndex, Bone bone)
        {
            if (!aibuBones.Contains(bone))
            {
                yield break;
            }
            float updateTimeSecs = 0.1f;
            float previousY = 0f;
            while (!game.Flags.isHSceneEnd)
            {
                var y = game.Flags.xy[aibuBones.IndexOf(bone)].y;
                if (previousY != y)
                {
                    HandleFondle(
                        y,
                        girlIndex,
                        bone: bone,
                        timeSecs: updateTimeSecs);
                    previousY = y;
                }
                yield return new WaitForSeconds(updateTimeSecs);
            }
        }
    }

    internal sealed class KoikatsuAibuStrokerController : KoikatsuAibuController
    {
        protected override void HandleFondle(float y, int girlIndex, Bone bone, float timeSecs) =>
            client.LinearCmd(y, timeSecs, girlIndex, bone);

        protected override void StopDevices(int girlIndex, Bone bone) {}
    }

    internal sealed class KoikatsuAibuVibratorController : KoikatsuAibuController
    {
        protected override void HandleFondle(float y, int girlIndex, Bone bone, float timeSecs) =>
            client.VibrateCmd(y, girlIndex, bone);

        protected override void StopDevices(int girlIndex, Bone bone) { }
    }
}
