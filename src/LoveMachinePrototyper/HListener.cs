using LoveMachine.Core.NonPortable;
using System.Collections;
using UnityEngine;

namespace LoveMachinePrototyper
{
    internal class HListener : CoroutineHandler
    {
        public void StartH() { }
        public void EndH() { }

        private void Start()
        {
            PrototyperConfig.ConfigChanged += (sender, args) => Restart();
            Restart();
        }

        private void Restart()
        {
            StopAllCoroutines();
            EndH();
            HandleCoroutine(ListenToH());
        }

        private IEnumerator ListenToH()
        {
            while (true)
            {
                Transform hStartObject;
                while ((hStartObject = GetHStartObject()) == null)
                {
                    yield return new WaitForSeconds(1f);
                }
                StartH();
                while (GetHStartObject() == hStartObject && hStartObject != null)
                {
                    yield return new WaitForSeconds(5f);
                }
                EndH();
            }
        }

        private Transform GetHStartObject()
        {
            return string.IsNullOrEmpty(PrototyperConfig.HStartObjectName.Value)
                ? FindUtil.FindFirst(PrototyperConfig.PenisBaseName.Value)
                : FindUtil.FindFirst(PrototyperConfig.HStartObjectName.Value);
        }
    }
}
