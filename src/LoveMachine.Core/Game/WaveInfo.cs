namespace LoveMachine.Core
{
    public struct WaveInfo
    {
        public float Phase { get; set; }
        public int Frequency { get; set; }
        public float Amplitude { get; set; }
        public float Preference { get; set; } // smaller is better
    }
}