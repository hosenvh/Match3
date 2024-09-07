using System;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Object = UnityEngine.Object;


namespace Match3.Presentation.TransitionEffects
{
    public interface TransitionEffect
    {
        void InstantiateTransitionEffectController(TransitionEffectControllerResourceAsset transitionEffectControllerResourceAsset);
        void StartTransitionEffect(Action onFadeInFinished, Action onFadeOutFinished);
    }

    public abstract class BaseTransitionEffect : TransitionEffect
    {
        
        private BaseTransitionEffectController transitionEffectController = default;

        protected BaseTransitionEffect()
        {
            Configure(ServiceLocator.Find<ConfigurationManager>());
        }

        public abstract void Configure(ConfigurationManager configurationManager);

        public void InstantiateTransitionEffectController(
            TransitionEffectControllerResourceAsset transitionEffectControllerResourceAsset)
        {
            transitionEffectController = Object.Instantiate(transitionEffectControllerResourceAsset.Load());
        }

        public void StartTransitionEffect(Action onFadeInFinished, Action onFadeOutFinished)
        {
            transitionEffectController.StartTransition(onFadeInFinished, () =>
            {
                onFadeOutFinished();
                Object.Destroy(transitionEffectController.gameObject);
            });
        }
        
    }
}


