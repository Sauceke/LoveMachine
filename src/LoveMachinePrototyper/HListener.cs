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
                while (getHStartObject() == null)
                {
                    yield return new WaitForSeconds(1f);
                }
                StartH();
                while (getHStartObject() != null)
                {
                    yield return new WaitForSeconds(5f);
                }
                EndH();
            }
        }

        private GameObject getHStartObject()
        {
            return string.IsNullOrEmpty(PrototyperConfig.HStartObjectName.Value)
                ? FindUtil.FindFirst<GameObject>(PrototyperConfig.PenisBaseName.Value)
                : FindUtil.FindFirst<GameObject>(PrototyperConfig.HStartObjectName.Value);
        }
    }
}
