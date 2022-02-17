namespace LoveMachine.Core
{
    public interface IDepthSensor
    {
        bool IsDeviceConnected { get; }

        bool TryGetNewDepth(bool peek, out float newDepth);
    }
}
