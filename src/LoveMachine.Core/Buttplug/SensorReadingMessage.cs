namespace LoveMachine.Core
{
    internal class SensorReadingMessage
    {
        public Reading SensorReading { get; set; }

        internal class Reading
        {
            public int DeviceIndex { get; set; }
            public string SensorType { get; set; }
            public int[] Data { get; set; }
        }
    }
}