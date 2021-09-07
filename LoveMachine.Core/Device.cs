using System.Collections.Generic;

namespace LoveMachine.Core
{
    public class Device
    {
        public string DeviceName { get; set; }
        public int DeviceIndex { get; set; }
        public int GirlIndex { get; set; } = 0;
        public int ActionIndex { get; set; } = 0;
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

    public struct DeviceSettings
    {
        public string DeviceName { get; private set; }
        public int GirlIndex { get; private set; }
        public int ActionIndex { get; private set; }

        public DeviceSettings(Device device)
        {
            DeviceName = device.DeviceName;
            GirlIndex = device.GirlIndex;
            ActionIndex = device.ActionIndex;
        }

        public void Apply(Device device)
        {
            device.GirlIndex = GirlIndex;
            device.ActionIndex = ActionIndex;
        }
    }
}
