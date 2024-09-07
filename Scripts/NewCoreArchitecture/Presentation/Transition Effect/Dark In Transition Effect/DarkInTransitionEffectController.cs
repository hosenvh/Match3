using System;
using UnityEngine;



namespace Match3.Presentation.TransitionEffects
{
    
    public class DarkInTransitionEffectController : BaseTransitionEffectController
    {
        [SerializeField] private GameObject darkImageGameObject = default;
    
        private Action onFadeInFinished;
        private Action onFadeOutFinished;
    
        private Animator animator;


        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        public override void StartTransition(Action onFadeInFinished, Action onFadeOutFinished)
        {
            this.onFadeInFinished = onFadeInFinished;
            this.onFadeOutFinished = onFadeOutFinished;
            FadeIn();
        }

        protected override void FadeIn()
        {
            darkImageGameObject.SetActive(true);
        }

        protected override void FadeOut()
        {
            // Nothing 
        }

        public void OnFadeInFinish()
        {
            animator.SetTrigger("changeFade");
            onFadeInFinished.Invoke();
        }

        public void OnFadeOutFinish()
        {
            darkImageGameObject.SetActive(false);
            onFadeOutFinished.Invoke();
        }
    }
    
}

