using System.Collections.Generic;

namespace LoveMachine.Core
{
    public class DeviceSettings
    {
        public string DeviceName { get; set; }
        public int GirlIndex { get; set; } = 0;
        public int BoneIndex { get; set; } = 0;
    }

    public class Device
    {
        public string DeviceName
        {
            get => Settings.DeviceName;
            set => Settings.DeviceName = value;
        }
        public int DeviceIndex { get; set; }
        public DeviceSettings Settings { get; set; } = new DeviceSettings();
        public Features DeviceMessages { get; set; }

        public bool IsVibrator { get { return DeviceMessages.VibrateCmd != null; } }
        public bool IsStroker { get { return DeviceMessages.LinearCmd != null; } }

        public class Features
        {
            public Command LinearCmd { get; set; }
            public Command VibrateCmd { get; set; }

            public class Command
            {
                public int FeatureCount { get; set; }
            }
        }
    }

    internal class DeviceListMessage
    {
        public DeviceListWrapper DeviceList { get; set; }

        internal class DeviceListWrapper
        {
            public List<Device> Devices { get; set; }
        }
    }
}
