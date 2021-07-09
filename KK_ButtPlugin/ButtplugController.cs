using System;
using System.Collections.Generic;
using System.Threading;

namespace KK_ButtPlugin
{
    public class ButtplugController
    {
        private static readonly List<HFlag.EMode> supportedModes = new List<HFlag.EMode>
        {
            HFlag.EMode.houshi,
            HFlag.EMode.sonyu
        };
        // animation -> fraction of normalized time at start of up-stroke
        // these were calibrated to the heroine's movements using PhaseDetector
        private static readonly Dictionary<string, float> animPhases =
            new Dictionary<string, float>
            {
                { "C. Doggy (Arm Pull).SLoop", 0.1522713f },
                { "C. Doggy (Arm Pull).WLoop", 0.4625397f },
                { "Chair Cowgirl.SLoop", 0.03100491f },
                { "Chair Cowgirl.WLoop", 0.9470091f },
                { "Chair Doggy.SLoop", 0.2003322f },
                { "Chair Doggy.WLoop", 0.9085472f },
                { "Chair Reverse Cowgirl.SLoop", 0.8580751f },
                { "Chair Reverse Cowgirl.WLoop", 0.4444504f },
                { "Cowgirl.SLoop", 0.2394352f },
                { "Cowgirl.WLoop", 0.9708462f },
                { "D. Doggy (Arm Pull).SLoop", 0.8819561f },
                { "D. Doggy (Arm Pull).WLoop", 0.966011f },
                { "Desk Doggy.SLoop", 0.2220459f },
                { "Desk Doggy.WLoop", 0.6546593f },
                { "Desk Missionary.SLoop", 0.1886158f },
                { "Desk Missionary.WLoop", 0.1895599f },
                { "Desk Side.SLoop", 0.06700228f },
                { "Desk Side.WLoop", 0.5270042f },
                { "Doggy (Arm Pull).SLoop", 0.1495857f },
                { "Doggy (Arm Pull).WLoop", 0.2061415f },
                { "Doggy (One Leg Up).SLoop", 0.2065086f },
                { "Doggy (One Leg Up).WLoop", 0.7616539f },
                { "Doggy.SLoop", 0.9896765f },
                { "Doggy.WLoop", 0.8349953f },
                { "Lotus (One Leg Up).SLoop", 0.3988791f },
                { "Lotus (One Leg Up).WLoop", 0.1558132f },
                { "Missionary Press.SLoop", 0.04641628f },
                { "Missionary Press.WLoop", 0.8453636f },
                { "Missionary.SLoop", 0.07053375f },
                { "Missionary.WLoop", 0.2240219f },
                { "Prone Doggy.SLoop", 0.293231f },
                { "Prone Doggy.WLoop", 0.6115961f },
                { "Side.SLoop", 0.2126694f },
                { "Side.WLoop", 0.003202677f },
                { "Spread Missionary.SLoop", 0.3389168f },
                { "Spread Missionary.WLoop", 0.9958129f },
                { "Standing Missionary.SLoop", 0.2895164f },
                { "Standing Missionary.WLoop", 0.3592472f },
                { "Standing.SLoop", 0.3157215f },
                { "Standing.WLoop", 0.1781301f }
            };

        private readonly ButtplugWsClient client = new ButtplugWsClient();
        private HFlag flags;
        private Thread loopThread;
        
        public void OnStart(HFlag flags)
        {
            this.flags = flags;
            if (loopThread != null)
            {
                return;
            }
            loopThread = new Thread(RunLoop)
            {
                Priority = ThreadPriority.AboveNormal
            };
            loopThread.Start();
        }

        public void OnFinish()
        {
            if (flags != null) {
                GetHeroine(flags).chaCtrl.animBody.speed = 1;
            }
            loopThread = null;
            flags = null;
            client.LinearCmd(0, 300);
        }

        public void OnAppQuit()
        {
            loopThread = null;
            client.Close();
        }

        private void RunLoop()
        {
            var animator = GetHeroine(flags).chaCtrl.animBody;
            var playerAnimator = flags.player.chaCtrl.animBody;
            double prevTime = double.MaxValue;
            while (loopThread != null)
            {
                if (!supportedModes.Contains(flags.mode))
                {
                    Thread.Sleep(1000);
                    continue;
                }
                var info = animator.GetCurrentAnimatorStateInfo(0);
                // nerf the animation speed so the device can keep up with it
                // OLoop is faster than the rest, about 280ms per stroke at its original speed
                playerAnimator.speed = animator.speed = info.IsName("OLoop")
                    ? GetSpeedMultiplierFor(0.28f)
                    : GetSpeedMultiplierFor(0.375f);
                double time = info.normalizedTime;
                string pose = flags.nowAnimationInfo.nameAnimation + "." + flags.nowAnimStateName;
                animPhases.TryGetValue(pose, out float phase);
                // sync stroke to animation loop starting over (thanks essu#1145 for the idea)
                if ((int)(time + 1 - phase) > (int)(prevTime + 1 - phase) && flags.speed >= 1)
                {
                    float strokeTimeSecs = info.length / info.speed;
                    int strokeTimeMs = (int)(strokeTimeSecs * 1000) - 10;
                    // decrease stroke length gradually as speed approaches the device limit
                    double rate = 60f / ButtPlugin.MaxStrokesPerMinute.Value / strokeTimeSecs;
                    double margin = rate * rate * 0.3;
                    if (info.IsName("OLoop"))
                    {
                        // no idea what's the deal with OLoop
                        // it seems to loop after two strokes
                        DoStroke(strokeTimeMs, margin);
                        Thread.Sleep(strokeTimeMs / 2);
                    }
                    DoStroke(strokeTimeMs, margin);
                }
                prevTime = time;
                Thread.Sleep(10);
            }
        }

        private float GetSpeedMultiplierFor(float animStrokeTimeSecs)
        {
            return Math.Min(1, animStrokeTimeSecs * ButtPlugin.MaxStrokesPerMinute.Value / 60f);
        }

        private void DoStroke(int strokeTimeMs, double margin)
        {
            client.LinearCmd(
                position: 1 - margin * 0.7,
                durationMs: strokeTimeMs / 2);
            Thread.Sleep(strokeTimeMs / 2);
            client.LinearCmd(
                position: margin * 0.3,
                durationMs: strokeTimeMs / 2);
            // skip sleep so we can react to speed changes
        }

        private static SaveData.Heroine GetHeroine(HFlag hflag)
        {
            return hflag.lstHeroine[0];
        }
    }
}
