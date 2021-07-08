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
        // animation -> normalized time from start of up-stroke to end of animation
        private static readonly Dictionary<string, float> animPhases =
            new Dictionary<string, float>
            {
                { "Missionary.WLoop", 0.45f },
                { "Missionary.SLoop", 0.45f },
                { "Cowgirl.SLoop", 0.63f },
                { "Chair Cowgirl.WLoop", 0.75f },
                { "Chair Cowgirl.SLoop", 0.5f },
                { "Standing.SLoop", 0.5f },
                { "Standing Missionary.SLoop", 0.5f },
                { "Standing Missionary.OLoop", 0.5f },
                { "Chair Reverse Cowgirl.WLoop", 0.45f },
                { "Chair Reverse Cowgirl.SLoop", 0.45f }
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
                if ((int)(time + phase) > (int)(prevTime + phase) && flags.speed >= 1)
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
