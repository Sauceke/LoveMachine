using LoveMachine.Core.Common;

namespace LoveMachine.Core.Buttplug.Settings
{
    public class FeatureSettings
    {
        public bool Enabled { get; set; } = true;
        public int GirlIndex { get; set; } = 0;
        public Bone Bone { get; set; } = Bone.Auto;
        public float PhaseShift { get; set; } = 0f;
        public Axis Axis { get; set; } = Axis.Longest;
        public MovementType MovementType { get; set; } = MovementType.Linear;
    }
}