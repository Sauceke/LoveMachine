using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.NonPortable;

namespace LoveMachine.Core.UI.Settings
{
    internal abstract class SettingsUI: CoroutineHandler
    {
        [HideFromIl2Cpp]
        public abstract void Draw(DeviceSettings deviceSettings);

        [HideFromIl2Cpp]
        public virtual void Draw(Device device) => Draw(device.Settings);
    }
}