using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public abstract class ClassicButtplugController : ButtplugController
    {
        protected abstract IEnumerator HandleAnimation(Device device, StrokeInfo strokeInfo);

        protected abstract IEnumerator HandleOrgasm(Device device);

        protected override IEnumerator Run(Device device)
        {
            while (true)
            {
                if (game.IsOrgasming(device.Settings.GirlIndex))
                {
                    var orgasm = HandleCoroutine(HandleOrgasm(device));
                    yield return new WaitForSecondsRealtime(game.MinOrgasmDurationSecs);
                    yield return WaitWhile(() => game.IsOrgasming(device.Settings.GirlIndex));
                    // unity may have destroyed the coroutine if it's already finished
                    if (orgasm != null)
                    {
                        StopCoroutine(orgasm);
                    }
                    continue;
                }
                if (IsIdleOrPaused(device))
                {
                    client.StopDeviceCmd(device);
                    while (IsIdleOrPaused(device))
                    {
                        yield return new WaitForSecondsRealtime(0.1f);
                    }
                    continue;
                }
                if (TryGetCurrentStrokeInfo(device, out var strokeInfo))
                {
                    yield return HandleCoroutine(HandleAnimation(device, strokeInfo));
                }
                else
                {
                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }
        }
        
        private bool IsIdleOrPaused(Device device) =>
            game.IsIdle(device.Settings.GirlIndex) || game.TimeScale == 0f;
    }
}