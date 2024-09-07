using System;
using UnityEngine;
using UnityEngine.Events;

namespace Match3.Presentation.Gameplay
{
    public class GeneralGameplayFeedBack : MonoBehaviour
    {
        public UnityEvent onPlay;
        Action onFinished = delegate { };

        public void Play()
        {
            onFinished = delegate { };
            Play(onFinished);
        }

        public void OnFinished()
        {
            Destroy(this.gameObject);
            onFinished();
            onFinished = delegate { };
        }

        public void Play(Action onFinished)
        {
            this.onFinished = onFinished;
            onPlay.Invoke();
        }
    }
}