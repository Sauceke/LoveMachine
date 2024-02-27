namespace LoveMachine.Core.Common
{
    public class RangeSetting
    {
        public RangeSetting()
        { }
        
        public RangeSetting(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float Min { get; set; }
        public float Max { get; set; }
    }
}