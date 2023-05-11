namespace LoveMachine.Core
{
    internal sealed class TestAnimationAnalyzer : AnimationAnalyzer
    {
        protected override bool TryGetResult(int girlIndex, Bone bone, out Result result)
        {
            result = new Result
            {
                StrokeDelimiters = new[] { 0f, 1f },
                Amplitude = 1f,
                Preference = 0f
            };
            return true;
        }
    }
}