using System.Collections;

namespace UnityEngine
{
    internal class WaitForSecondsRealtime : IEnumerator
    {
        private float seconds;

        public WaitForSecondsRealtime(float seconds) => this.seconds = seconds;

        public object Current => new WaitForSeconds(seconds);

        public bool MoveNext() => false;

        public void Reset()
        { }
    }
}