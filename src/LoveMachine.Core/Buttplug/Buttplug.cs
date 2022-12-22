using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoveMachine.Core
{
    public class Buttplug
    {
        private static int NewId => UnityEngine.Random.Range(0, int.MaxValue);

        public static object RequestServerInfo() => new
        {
            RequestServerInfo = new
            {
                Id = NewId,
                ClientName = Paths.ProcessName,
                MessageVersion = 3
            }
        };

        public static object RequestDeviceList() => new
        {
            RequestDeviceList = new
            {
                Id = NewId
            }
        };

        public static object StartScan() => new
        {
            StartScanning = new
            {
                Id = NewId
            }
        };

        public static object StopScan() => new
        {
            StopScanning = new
            {
                Id = NewId
            }
        };

        public static object StopDeviceCmd(Device device) => new
        {
            StopDeviceCmd = new
            {
                Id = NewId,
                DeviceIndex = device.DeviceIndex
            }
        };

        public static object StopAllDevices() => new
        {
            StopAllDevices = new
            {
                Id = NewId
            }
        };

        public static object LinearCmd(Device device, float position, float durationSecs) => new
        {
            LinearCmd = new
            {
                Id = NewId,
                DeviceIndex = device.DeviceIndex,
                Vectors = device.DeviceMessages.LinearCmd
                    .Select((feature, featureIndex) => new
                    {
                        Index = featureIndex,
                        Duration = (int)(durationSecs * 1000f),
                        Position = position
                    })
                    .ToArray()
            }
        };

        public static object ScalarCmd(Device device, float value, string actuatorType) => new
        {
            ScalarCmd = new
            {
                Id = NewId,
                DeviceIndex = device.DeviceIndex,
                Scalars = device.DeviceMessages.ScalarCmd
                    .Select((feature, featureIndex) => new
                    {
                        Index = featureIndex,
                        Scalar = value,
                        ActuatorType = feature.ActuatorType
                    })
                    .Where(cmd => cmd.ActuatorType == actuatorType)
                    .ToArray()
            }
        };

        public static object RotateCmd(Device device, float speed, bool clockwise) => new
        {
            RotateCmd = new
            {
                Id = NewId,
                DeviceIndex = device.DeviceIndex,
                Rotations = device.DeviceMessages.RotateCmd
                    .Select((feature, featureIndex) => new
                    {
                        Index = featureIndex,
                        Speed = speed,
                        Clockwise = clockwise
                    })
                    .ToArray()
            }
        };

        public static object BatteryLevelCmd(Device device) => new
        {
            SensorReadCmd = new
            {
                Id = NewId,
                DeviceIndex = device.DeviceIndex,
                SensorIndex = Array.FindIndex(
                    device.DeviceMessages.SensorReadCmd, f => f.HasBatteryLevel),
                SensorType = Feature.Battery
            }
        };

        public class SensorReadingMessage
        {
            public SensorReading SensorReading { get; set; }
        }

        public class SensorReading
        {
            public int DeviceIndex { get; set; }
            public string SensorType { get; set; }
            public int[] Data { get; set; }
        }

        public class Device
        {
            public virtual string DeviceName { get; set; }
            public int DeviceIndex { get; set; }
            public Features DeviceMessages { get; set; }
        }

        public class Features
        {
            public Feature[] LinearCmd { get; set; }
            public Feature[] ScalarCmd { get; set; }
            public Feature[] RotateCmd { get; set; }
            public Feature[] SensorReadCmd { get; set; }
        }

        public class Feature
        {
            internal const string Vibrate = "Vibrate";
            internal const string Constrict = "Constrict";
            internal const string Battery = "Battery";

            public string ActuatorType { get; set; }
            public string SensorType { get; set; }

            public bool IsVibrator => ActuatorType == Vibrate;
            public bool IsConstrictor => ActuatorType == Constrict;
            public bool HasBatteryLevel => SensorType == Battery;
        }

        public class DeviceListMessage<D>
            where D : Device
        {
            public DeviceList<D> DeviceList { get; set; }
        }

        public class DeviceList<D>
            where D : Device
        {
            public List<D> Devices { get; set; }
        }
    }
}