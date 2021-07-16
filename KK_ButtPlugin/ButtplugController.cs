using System;
using System.Collections;
using System.Collections.Generic;
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
        private static readonly Dictionary<string, float> animPhases =
            new Dictionary<string, float>
            {
                { "Chair Cowgirl.WLoop.0", 0.25f },
                { "Chair Cowgirl.SLoop.0", 0.5199027f },
                { "Chair Cowgirl.OLoop.0", 0.9880123f },
                { "Chair Reverse Cowgirl.WLoop.0", 0.4341354f },
                { "Chair Reverse Cowgirl.SLoop.0", 0.9055362f },
                { "Chair Reverse Cowgirl.OLoop.0", 0.9685769f },
                { "Chair Doggy.WLoop.0", 0.8953152f },
                { "Chair Doggy.SLoop.0", 0.1751766f },
                { "Chair Doggy.OLoop.0", 0.9192567f },
                { "C. Doggy (Arm Pull).WLoop.0", 0.09556512f },
                { "C. Doggy (Arm Pull).SLoop.0", 0.9297943f },
                { "C. Doggy (Arm Pull).OLoop.0", 0.3201385f },
                { "Missionary.WLoop.0", 0.2155571f },
                { "Missionary.SLoop.0", 0.1671515f },
                { "Missionary.OLoop.0", 0.9366393f },
                { "Spread Missionary.WLoop.0", 0.9430294f },
                { "Spread Missionary.SLoop.0", 0.2694383f },
                { "Spread Missionary.OLoop.0", 0.0840826f },
                { "Doggy.WLoop.0", 0.5001087f },
                { "Doggy.SLoop.0", 0.1849718f },
                { "Doggy.OLoop.0", 0.957346f },
                { "Doggy (Arm Pull).WLoop.0", 0.3310409f },
                { "Doggy (Arm Pull).SLoop.0", 0.1534224f },
                { "Doggy (Arm Pull).OLoop.0", 0.8189626f },
                { "Cowgirl.WLoop.0", 0.9488726f },
                { "Cowgirl.SLoop.0", 0.2407475f },
                { "Cowgirl.OLoop.0", 0.1484954f },
                { "Side.WLoop.0", 0.8933277f },
                { "Side.SLoop.0", 0.1974277f },
                { "Side.OLoop.0", 0f },
                { "Standing.WLoop.0", 0.2255281f },
                { "Standing.SLoop.0", 0.4492655f },
                { "Standing.OLoop.0", 0.8939435f },
                { "Standing Missionary.WLoop.0", 0.2304764f },
                { "Standing Missionary.SLoop.0", 0.4580402f },
                { "Standing Missionary.OLoop.0", 0.04387951f },
                { "Missionary Press.WLoop.0", 0.07942724f },
                { "Missionary Press.SLoop.0", 0.9985008f },
                { "Missionary Press.OLoop.0", 0.1289151f },
                { "Prone Doggy.WLoop.0", 0.8515744f },
                { "Prone Doggy.SLoop.0", 0.1763554f },
                { "Prone Doggy.OLoop.0", 0.3384476f },
                { "Desk Missionary.WLoop.0", 0.2367048f },
                { "Desk Missionary.SLoop.0", 0.9518476f },
                { "Desk Missionary.OLoop.0", 0.9380744f },
                { "Desk Doggy.WLoop.0", 0.426898f },
                { "Desk Doggy.SLoop.0", 0.1048675f },
                { "Desk Doggy.OLoop.0", 0.8736467f },
                { "D. Doggy (Arm Pull).WLoop.0", 0.5098515f },
                { "D. Doggy (Arm Pull).SLoop.0", 0.4909382f },
                { "D. Doggy (Arm Pull).OLoop.0", 0.8721614f },
                { "Desk Side.WLoop.0", 0.5578098f },
                { "Desk Side.SLoop.0", 0.8853645f },
                { "Desk Side.OLoop.0", 0.105258f },
                { "Doggy (One Leg Up).WLoop.0", 0.2770634f },
                { "Doggy (One Leg Up).SLoop.0", 0.1949372f },
                { "Doggy (One Leg Up).OLoop.0", 0.9595118f },
                { "Lotus (One Leg Up).WLoop.0", 0.2271376f },
                { "Lotus (One Leg Up).SLoop.0", 0.4290304f },
                { "Lotus (One Leg Up).OLoop.0", 0.9597445f },
                // 3P
                { "Cowgirl Cunnilingus.WLoop.0", 0f },
                { "Cowgirl Cunnilingus.SLoop.0", 0.9995918f },
                { "Cowgirl Cunnilingus.OLoop.0", 0.87395f },
                { "Cowgirl Cunnilingus (Alt).WLoop.0", 0.25f },
                { "Cowgirl Cunnilingus (Alt).SLoop.0", 0f },
                { "Cowgirl Cunnilingus (Alt).OLoop.0", 0.9146178f },
                { "Cowgirl Cunnilingus.WLoop.1", 0.25f },
                { "Cowgirl Cunnilingus.SLoop.1", 0f },
                { "Cowgirl Cunnilingus.OLoop.1", 0.87395f },
                { "Cowgirl Cunnilingus (Alt).WLoop.1", 0.05144724f },
                { "Cowgirl Cunnilingus (Alt).SLoop.1", 0.973187f },
                { "Cowgirl Cunnilingus (Alt).OLoop.1", 0.9305124f }
            };

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
            ButtPlugin.Logger.LogDebug("ST " + strokeTimeMs +" " + margin + " " + girlIndex);
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
