namespace LoveMachine.Core
{
    internal sealed class TestAnimationAnalyzer : AnimationAnalyzer
    {
        protected override bool TryGetWaveInfo(int girlIndex, Bone bone, out WaveInfo result)
        {
            result = new WaveInfo
            {
                StrokeDelimiters = new[] { 0f, 1f },
                Amplitude = 1f,
                Preference = 0f
            };
            return true;
        }
    }
}