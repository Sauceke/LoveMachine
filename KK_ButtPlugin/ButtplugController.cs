using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace KK_ButtPlugin
{
    public class ButtplugController : MonoBehaviour
    {
        private static readonly List<HFlag.EMode> supportedModes = new List<HFlag.EMode>
        {
            HFlag.EMode.houshi, HFlag.EMode.sonyu, HFlag.EMode.houshi3P, HFlag.EMode.sonyu3P
        };

        private static readonly List<string> orgasmAnimations = new List<string>
        {
            "OLoop", "A_OLoop",

            // ejaculation
            "OUT_START", "OUT_LOOP", "IN_START", "IN_LOOP",
            "M_OUT_Start", "M_OUT_Loop", "M_IN_Start", "M_IN_Loop",
            "WS_IN_Start", "WS_IN_Loop", "SS_IN_Start", "SS_IN_Loop",
            "A_WS_IN_Start", "A_WS_IN_Loop", "A_SS_IN_Start", "A_SS_IN_Loop",

            // insertion excitement
            "Pull", "A_Pull", "Insert", "A_Insert"
        };

        private static readonly List<string> supportedAnimations = new List<string>
        {
            "WLoop", "SLoop",
            // masturbation
            "MLoop", 
            // anal
            "A_WLoop", "A_SLoop", "A_OLoop",

            // orgasm
            "OLoop", "A_OLoop",
            "OUT_START", "OUT_LOOP", "IN_START", "IN_LOOP",
            "M_OUT_Start", "M_OUT_Loop", "M_IN_Start", "M_IN_Loop",
            "WS_IN_Start", "WS_IN_Loop", "SS_IN_Start", "SS_IN_Loop",
            "A_WS_IN_Start", "A_WS_IN_Loop", "A_SS_IN_Start", "A_SS_IN_Loop",
            
            // insertion
            "Pull", "A_Pull", "Insert", "A_Insert"
        };

        
        

        // animation -> fractional part of normalized time at start of up-stroke
        private Dictionary<string, float> animPhases;

        private readonly ButtplugWsClient client = new ButtplugWsClient();
        private HFlag flags;

        public List<Device> Devices
        {
            get { return client.Devices; }
        }

        public bool IsConnected
        {
            get { return client.IsConnected; }
        }

        public bool IsOrgasm
        {
            get { return orgasmAnimations.Contains(flags.nowAnimStateName); }
        }


        public void Awake()
        {
            string animConfigPath = Path.GetDirectoryName(ButtPlugin.Info.Location)
                + Path.DirectorySeparatorChar
                + "animations.json";
            string animConfigJson = File.ReadAllText(animConfigPath);
            animPhases = JsonMapper.ToObject<Dictionary<string, float>>(animConfigJson);
        }

        public void OnStartH(HFlag flags)
        {
            this.flags = flags;
            StartCoroutine(RunLoops());
        }

        IEnumerator RunLoops()
        {
            yield return StartCoroutine(UntilReady());
            for (int i = 0; i < flags.lstHeroine.Count; i++)
            {
                StartCoroutine(RunStroke(girlIndex: i));
            }
            StartCoroutine(RunVibrate());
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            client.Close();
        }

        public void Connect()
        {
            client.Close(); // close previous connection just in case
            client.Open();
            Scan();
        }

        public void Scan()
        {
            StartCoroutine("ScanDevices");
        }

        private string GetPose(int girlIndex = 0)
        {
            return flags.nowAnimationInfo.nameAnimation
                + "." + flags.nowAnimStateName
                + "." + girlIndex;
        }

        private float GetPhase(int girlIndex = 0)
        {
            if (!animPhases.TryGetValue(GetPose(girlIndex), out float phase))
            {
                return 0.0f;  // 
            }
            return phase;
        }

        IEnumerator ScanDevices()
        {
            client.StartScan();
            yield return new WaitForSeconds(15.0f);
            client.StopScan();
        }

        IEnumerator UntilReady()
        {
            while (flags.lstHeroine.IsNullOrEmpty()
                || flags.lstHeroine.Any(girl => girl.chaCtrl?.animBody == null)
                || flags.player?.chaCtrl?.animBody == null)
            {
                yield return new WaitForSeconds(1f);
                if (flags.isHSceneEnd)
                {
                    yield break;
                }
            }
        }

        IEnumerator RunVibrate()
        {
            while (!flags.isHSceneEnd)
            {
                if (!supportedModes.Contains(flags.mode) || !supportedAnimations.Contains(flags.nowAnimStateName))
                {
                    // stops vibration when not being lewd
                    DoVibrate(0.0f);
                    yield return new WaitForSecondsRealtime(1.0f / (float)ButtPlugin.VibrationFrequency.Value);
                    continue;
                }

                var animator = flags.lstHeroine[0].chaCtrl.animBody;

                var speed = flags.speedCalc;
                var strength = 1.0f;
                var minVibration = 0.2f;

                // service mode goes into OLoop once male excitement exceeds its threshold
                if (IsOrgasm)
                {
                    speed = 1.0f;
                    minVibration = 0.6f;
                }

                if (ButtPlugin.SyncVibrationWithAnimation.Value)
                {
                    // Simple sin based intensity amplification based on normalized position in looping animation
                    var info = animator.GetCurrentAnimatorStateInfo(0);
                    var depth = (info.m_NormalizedTime - GetPhase()) % 1;
                    strength = Mathf.Sin(Mathf.Lerp(0, Mathf.PI, depth)) + 0.1f;
                }

                DoVibrate(Mathf.Lerp(minVibration, 1.0f, speed * strength));
                yield return new WaitForSecondsRealtime(1.0f / (float)ButtPlugin.VibrationFrequency.Value);
            }
            // turn off vibration since there's nothing to animate against
            // this state can happen if H is ended while the animation is not in Idle
            DoVibrate(0.0f);
        }

        IEnumerator RunStroke(int girlIndex)
        {
            var animator = flags.lstHeroine[girlIndex].chaCtrl.animBody;
            var playerAnimator = flags.player.chaCtrl.animBody;
            double prevNormTime = double.MaxValue;
            while (!flags.isHSceneEnd)
            {
                if (!supportedModes.Contains(flags.mode)
                    || !supportedAnimations.Contains(flags.nowAnimStateName)
                    || flags.speed < 1)
                {
                    yield return new WaitForSeconds(.1f);
                    continue;
                }
                var info = animator.GetCurrentAnimatorStateInfo(0);
                // nerf the animation speed so the device can keep up with it
                // OLoop is faster than the rest, about 280ms per stroke at its original speed
                playerAnimator.speed = animator.speed = info.IsName("OLoop")
                    ? GetSpeedMultiplierFor(0.28f)
                    : GetSpeedMultiplierFor(0.375f);
                double normTime = info.normalizedTime;
                float phase = GetPhase(girlIndex);
                float strokeTimeSecs = info.length / info.speed;
                // sometimes info.length becomes Infinity for some reason
                // this is a catch-all for god knows what other horrors possibly lurking
                // in the game that would make this loop hang
                if (strokeTimeSecs > 10)
                {
                    yield return new WaitForSeconds(.01f);
                    continue;
                }
                float latencyNormTime = ButtPlugin.LatencyMs.Value / 1000f / strokeTimeSecs;
                phase -= latencyNormTime;
                // sync stroke to animation loop starting over (thanks essu#1145 for the idea)
                if ((int)(normTime - phase + 2) > (int)(prevNormTime - phase + 2))
                {
                    int strokeTimeMs = (int)(strokeTimeSecs * 1000) - 10;
                    // decrease stroke length gradually as speed approaches the device limit
                    double rate = 60f / ButtPlugin.MaxStrokesPerMinute.Value / strokeTimeSecs;
                    double margin = rate * rate * 0.3;
                    if (info.IsName("OLoop"))
                    {
                        // no idea what's the deal with OLoop
                        // it seems to loop after two strokes
                        yield return StartCoroutine(DoStroke(strokeTimeMs, margin, girlIndex));
                        yield return new WaitForSeconds(strokeTimeMs / 2000f);
                    }
                    yield return StartCoroutine(DoStroke(strokeTimeMs, margin, girlIndex));
                }
                prevNormTime = normTime;
                yield return new WaitForSeconds(.01f);
            }
        }

        private float GetSpeedMultiplierFor(float animStrokeTimeSecs)
        {
            return Math.Min(1, animStrokeTimeSecs * ButtPlugin.MaxStrokesPerMinute.Value / 60f);
        }

        IEnumerator DoStroke(int strokeTimeMs, double margin, int girlIndex)
        {
            client.LinearCmd(
                position: 1 - margin * 0.7,
                durationMs: strokeTimeMs / 2,
                girlIndex);
            yield return new WaitForSeconds(strokeTimeMs / 2000f);
            client.LinearCmd(
                position: margin * 0.3,
                durationMs: strokeTimeMs / 2,
                girlIndex);
        }

        private void DoVibrate(float intensity)
        {
            if (!ButtPlugin.EnableVibrate.Value)
            {
                return;
            }
            client.VibrateCmd(intensity);
        }
    }
}
