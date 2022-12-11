using BepInEx;
using System;
using System.Linq;

namespace LoveMachine.Core
{
    internal class Buttplug
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
                SensorType = Device.Features.Feature.Battery
            }
        };
    }
}