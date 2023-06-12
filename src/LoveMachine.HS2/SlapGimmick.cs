using System.Collections;
using System.Linq;
using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Extras;

namespace LoveMachine.HS2
{
    public class SlapGimmick : Gimmick
    {
        private readonly string[] slapAnimationNames = { "SAction", "WAction" };
        
        protected override IEnumerator Run(Device device)
        {
            var hs2 = GetComponent<HoneySelect2Game>();
            float updateTimeSecs = 1f / device.Settings.UpdatesHz;
            while (true)
            {
                yield return new WaitForSecondsRealtime(updateTimeSecs);
                var info = hs2.animators[device.Settings.GirlIndex]
                    .GetCurrentAnimatorStateInfo(0);
                if (slapAnimationNames.Any(info.IsName))
                {
                    yield return DoStroke(device, info.length);
                    yield return new WaitForSecondsRealtime(info.length / 2);
                }
            }
        }
    }
}