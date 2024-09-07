using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace PandasCanPlay.HexaWord.Utility
{

    public class DelayHandler : MonoBehaviour
    {
        [FormerlySerializedAs("name")]
        public string id;
		public float duration;
        public bool autoStart = false;

        public UnityEvent timeoutEvent;


        void Start()
        {
            if (autoStart)
                StartTimer();
        }

		public void StartTimer()
		{
			StopAllCoroutines();
			StartCoroutine(WaitFor(duration));
		}

        public void StopTimer()
        {
            StopAllCoroutines();
        }


		private IEnumerator WaitFor(float duration)
		{
			yield return new WaitForSeconds(duration);
			timeoutEvent.Invoke ();
		}

	}
}