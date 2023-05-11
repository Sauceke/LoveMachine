namespace LoveMachine.Core
{
    internal sealed class TestAnimationAnalyzer : AnimationAnalyzer
    {
        public TestAnimationAnalyzer(GameDescriptor game) => this.game = game;
        
        protected override bool TryGetResult(int girlIndex, Bone bone, out Result result)
        {
            result = new Result
            {
                StrokeDelimiters = new[] { 0f },
                Amplitude = 1f,
                Preference = 0f
            };
            return true;
        }
    }
}