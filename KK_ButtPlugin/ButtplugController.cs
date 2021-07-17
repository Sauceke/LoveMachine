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
        private static readonly List<string> supportedAnimations = new List<string>
        {
            "WLoop", "SLoop", "OLoop"
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
                if (flags.nowAnimStateName.Equals("OLoop"))
                {
                    DoVibrate(1.0f);
                }
                else if (!supportedModes.Contains(flags.mode) || !supportedAnimations.Contains(flags.nowAnimStateName))
                {
                    // stops vibration when not being lewd
                    DoVibrate(0.0f);
                }
                else
                {
                    // vibrate based on the intensity of the player
                    // minimum vibration above 0 exists so you always feel something along with the animation
                    DoVibrate(Mathf.Lerp(0.2f, 1.0f, flags.speedCalc));
                }
                yield return new WaitForSeconds(.01f);
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
                string pose = flags.nowAnimationInfo.nameAnimation
                    + "." + flags.nowAnimStateName
                    + "." + girlIndex;
                animPhases.TryGetValue(pose, out float phase);
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
                    ButtPlugin.Logger.LogDebug(phase);
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
