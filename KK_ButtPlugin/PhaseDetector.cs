using IllusionUtility.GetUtility;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KK_ButtPlugin
{
    // measuring tool for syncing toys to animations
    class PhaseDetector
    {
        public static IEnumerator RunLoop(HFlag flags, int girlIndex)
        {
            yield return new WaitForSeconds(10f);
            var animator = flags.lstHeroine[girlIndex].chaCtrl.animBody;
            var crotch1 = flags.lstHeroine[girlIndex].chaCtrl.objBodyBone.transform
                .FindLoop("cf_n_pee").transform;
            var crotch2 = flags.player.chaCtrl.objBodyBone.transform
                .FindLoop("k_f_tamaL_00").transform;
            Dictionary<string, float> minDistSqTimes = new Dictionary<string, float> { };
            Dictionary<string, float> minDistSqs = new Dictionary<string, float> { };
            flags.player.chaCtrl.animBody.speed = 0;
            while (!flags.isHSceneEnd) {
                yield return new WaitForSeconds(0.01f);
                if (!flags.nowAnimStateName.EndsWith("Loop"))
                {
                    continue;
                }
                var distSq = crotch1.position.y;
                // uncomment for measuring distance to pp
                // distSq = (crotch1.position - crotch2.position).sqrMagnitude;
                string pose = flags.nowAnimationInfo.nameAnimation + "." + flags.nowAnimStateName
                    + "." + girlIndex;
                minDistSqs.TryGetValue(pose, out float minDistSq);
                if (distSq < minDistSq || minDistSq == 0)
                {
                    var info = animator.GetCurrentAnimatorStateInfo(0);
                    float time = info.normalizedTime % 1f;
                    minDistSqTimes.TryGetValue(pose, out float minDistSqTime);
                    // trying to filter out the BS that comes in when changing poses
                    if (Math.Abs(time - minDistSqTime) > 0.1
                        && Math.Abs(time - minDistSqTime) < 0.9)
                    {
                        continue;
                    }
                    minDistSqs[pose] = distSq;
                    minDistSqTimes[pose] = time;
                    ButtPlugin.Logger.LogDebug(JsonMapper.ToJson(minDistSqTimes));
                }
            }
        }
    }
}
