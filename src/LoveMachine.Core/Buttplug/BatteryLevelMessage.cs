namespace LoveMachine.Core
{
    internal class BatteryLevelMessage
    {
        public BatteryReading BatteryLevelReading { get; set; }

        internal class BatteryReading
        {
            public int Id { get; set; }
            public int DeviceIndex { get; set; }
            public float BatteryLevel { get; set; }
        }
    }
}