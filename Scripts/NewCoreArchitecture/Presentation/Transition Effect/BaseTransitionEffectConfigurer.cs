using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using Match3.Presentation.TransitionEffects;

namespace Match3.TransitionEffect.Data
{
    public abstract class BaseTransitionEffectConfigurer <T> : ScriptableConfiguration, Configurer<T> where T : Presentation.TransitionEffects.TransitionEffect
    {
        public TransitionEffectControllerResourceAsset darkTransitionEffectControllerResourceAsset;

        public void Configure(T transitionEffect)
        {
            transitionEffect.InstantiateTransitionEffectController(darkTransitionEffectControllerResourceAsset);
        }

        public abstract override void RegisterSelf(ConfigurationManager manager);
    }
}