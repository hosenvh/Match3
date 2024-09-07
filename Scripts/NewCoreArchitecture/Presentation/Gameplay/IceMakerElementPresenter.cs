using System;
using UnityEngine;
using UnityEngine.Events;


namespace Match3.Presentation.Gameplay
{
    public class IceMakerElementPresenter : MonoBehaviour
    {
        [SerializeField] private AnimationPlayer animationPlayer;
        [SerializeField] private AnimationClip popingAnimationClip;
        [SerializeField] private AnimationClip popingEndAnimationClip;
        [Space]
        [SerializeField] private UnityEvent onPopingStarted;
        [SerializeField] private UnityEvent onPopingEnd;

        public void PlayPopingEffect()
        {
            onPopingStarted.Invoke();
            animationPlayer.Play(popingAnimationClip);
        }

        public void PlayDestroyEffect(Action onComplete)
        {
            animationPlayer.Play(popingEndAnimationClip, () =>
            {
                onComplete();
                Destroy(gameObject, Time.deltaTime);
            });
            onPopingEnd.Invoke();
        }
    }
}
