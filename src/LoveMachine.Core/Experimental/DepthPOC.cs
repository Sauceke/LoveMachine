using UnityEngine;

namespace LoveMachine.Core
{
    public class DepthPOC : MonoBehaviour
    {
        public bool IsDeviceConnected { get; protected set; }

        public float Depth { get; protected set; }
    }
}
