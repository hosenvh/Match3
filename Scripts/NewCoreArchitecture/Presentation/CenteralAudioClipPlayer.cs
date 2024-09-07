using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.Gameplay;
using UnityEngine;


namespace Match3.Presentation
{
    public class CenteralAudioClipPlayer : MonoBehaviour
    {
        public AudioClip clip;
        public bool loop;

        GameplaySoundManager gameplaySoundManager;

        void Awake()
        {
            gameplaySoundManager = ServiceLocator.Find<GameplaySoundManager>();
        }

        public void Play()
        {
            gameplaySoundManager.TryPlay(clip, loop);
        }

        public void Stop()
        {
            gameplaySoundManager.TryStop(clip);
        }

        private void OnDestroy()
        {
            gameplaySoundManager.TryStop(clip);
        }
    }
}