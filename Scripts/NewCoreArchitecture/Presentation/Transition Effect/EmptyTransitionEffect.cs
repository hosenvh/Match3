using System;


namespace Match3.Presentation.TransitionEffects
{

    public class EmptyTransitionEffect : TransitionEffect
    {
        public void InstantiateTransitionEffectController(
            TransitionEffectControllerResourceAsset transitionEffectControllerResourceAsset)
        {
            // Nothing for this one
        }

        public void StartTransitionEffect(Action onFadeInFinished, Action onFadeOutFinished)
        {
            onFadeInFinished.Invoke();
        }
    }
    
}


