using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoveMachine.Core.Buttplug.Settings
{
    public class OscillateSettings
    {
        public bool Enabled { get; set; } = true;
        public float SpeedMin { get; set; } = 0f;
        public float SpeedMax { get; set; } = 1f;
        public float SpeedSensitivityMin { get; set; } = 1f;
        public float SpeedSensitivityMax { get; set; } = 3f;
        public int UpdateIntervalSecs { get; set; } = 5;
    }
}
