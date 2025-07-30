using LoveMachine.Core.Common;

namespace LoveMachine.Core.Game
{
    public struct TrackingKey
    {
        public int GirlIndex { get; set; }
        public Bone Bone { get; set; }
        public string Pose { get; set; }
        public POV POV { get; set; }
        public Axis Axis { get; set; }
        public MovementType MovementType { get; set; }
    }
}