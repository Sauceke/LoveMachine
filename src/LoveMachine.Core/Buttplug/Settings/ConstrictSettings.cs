namespace LoveMachine.Core.Buttplug.Settings
{
    public class ConstrictSettings
    {
        public bool Enabled { get; set; } = true;
        public float PressureMin { get; set; } = 0f;
        public float PressureMax { get; set; } = 1f;
        public float SpeedSensitivityMin { get; set; } = 1f;
        public float SpeedSensitivityMax { get; set; } = 3f;
        public int UpdateIntervalSecs { get; set; } = 5;
    }
}