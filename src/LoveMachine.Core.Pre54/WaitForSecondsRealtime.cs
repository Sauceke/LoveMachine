using System.Collections;

namespace UnityEngine
{
    internal class WaitForSecondsRealtime : IEnumerator
    {
        private readonly float seconds;
        private bool done = false;

        public WaitForSecondsRealtime(float seconds) => this.seconds = seconds;

        public object Current => new WaitForSeconds(seconds);

        public bool MoveNext() => !done && (done = true);

        public void Reset() => done = false;
    }
}