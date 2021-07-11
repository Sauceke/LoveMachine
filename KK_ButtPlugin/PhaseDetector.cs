using IllusionUtility.GetUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace KK_ButtPlugin
{
    // measuring tool for syncing toys to animations
    class PhaseDetector
    {
        public static void RunLoop(HFlag flags)
        {
            var animator = GetHeroine(flags).chaCtrl.animBody;
            var crotch1 = GetHeroine(flags).chaCtrl.objBodyBone.transform
                .FindLoop("cf_n_pee").transform;
            var crotch2 = flags.player.chaCtrl.objBodyBone.transform
                .FindLoop("k_f_tamaL_00").transform;
            Dictionary<string, float> minDistSqTimes = new Dictionary<string, float> { };
            Dictionary<string, float> minDistSqs = new Dictionary<string, float> { };
            flags.player.chaCtrl.animBody.speed = 0;
            while (true) {
                Thread.Sleep(10);
                if (!flags.nowAnimStateName.EndsWith("Loop"))
                {
                    continue;
                }
                var distSq = (crotch1.position - crotch2.position).sqrMagnitude;
                string pose = flags.nowAnimationInfo.nameAnimation + "." + flags.nowAnimStateName;
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
                    ButtPlugin.Logger.LogDebug(DictToString(minDistSqTimes));
                }
            }
        }

        private static SaveData.Heroine GetHeroine(HFlag hflag)
        {
            return hflag.lstHeroine[0];
        }

        private static string DictToString<K, V>(Dictionary<K, V> dict)
        {
            return dict
                .Select(pair => "{ \"" + pair.Key + "\", " + pair.Value + "f }")
                .Aggregate("", (a, b) => a + ",\r\n" + b);
        }
    }
}
