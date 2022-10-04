using System;

namespace LoveMachine.Core
{
    internal sealed class TestAnimationAnalyzer : AnimationAnalyzer
    {
        public TestAnimationAnalyzer(IntPtr handle) : base(handle) { }

        public override bool TryGetWaveInfo(int girlIndex, Bone bone, out WaveInfo result)
        {
            result = new WaveInfo
            {
                Phase = 0f,
                Frequency = 1,
                Amplitude = 1f,
                Preference = 0f
            };
            return true;
        }
    }
}
