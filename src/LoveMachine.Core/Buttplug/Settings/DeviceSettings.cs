namespace LoveMachine.Core.Buttplug.Settings
{
    public class DeviceSettings
    {
        public string DeviceName { get; set; }
        public int LatencyMs { get; set; } = 0;
        public int UpdatesHz { get; set; } = 10;
        public StrokerSettings StrokerSettings { get; set; } = new StrokerSettings();
        public VibratorSettings VibratorSettings { get; set; } = new VibratorSettings();
        public ConstrictSettings ConstrictSettings { get; set; } = new ConstrictSettings();
        public FeatureSettings GlobalFeatureSettings { get; set; } = new FeatureSettings();
        public FeatureSettings[] LinearCmdSettings { get; set; } = new FeatureSettings[0];
        public FeatureSettings[] RotateCmdSettings { get; set; } = new FeatureSettings[0];
        public FeatureSettings[] ScalarCmdSettings { get; set; } = new FeatureSettings[0];
        public bool UseSeparateFeatureSettings { get; set; } = false;
    }
}
