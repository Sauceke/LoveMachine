using System.Collections;
using UnityEngine;

namespace LoveMachine.Core
{
    public abstract class ClassicButtplugController : ButtplugController
    {
        protected abstract IEnumerator HandleAnimation(Device device, WaveInfo waveInfo);

        protected abstract IEnumerator HandleOrgasm(Device device);

        protected override IEnumerator Run(Device device)
        {
            while (true)
            {
                if (game.IsOrgasming(device.Settings.GirlIndex))
                {
                    yield return HandleCoroutine(HandleOrgasm(device));
                    continue;
                }
                if (game.IsIdle(device.Settings.GirlIndex))
                {
                    client.StopDeviceCmd(device);
                    while (game.IsIdle(device.Settings.GirlIndex))
                    {
                        yield return new WaitForSecondsRealtime(0.1f);
                    }
                    continue;
                }
                int girlIndex = device.Settings.GirlIndex;
                var bone = device.Settings.Bone;
                if (analyzer.TryGetWaveInfo(girlIndex, bone, out var waveInfo))
                {
                    yield return HandleCoroutine(HandleAnimation(device, waveInfo));
                }
                else
                {
                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }
        }
    }
}