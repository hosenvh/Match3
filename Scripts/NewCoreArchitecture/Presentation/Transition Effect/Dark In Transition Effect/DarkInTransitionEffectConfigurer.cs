using Match3.Foundation.Base.Configuration;
using Match3.Presentation.TransitionEffects;
using UnityEngine;


namespace Match3.TransitionEffect.Data
{
    
    [CreateAssetMenu(menuName = "Match3/Configurations/TransitionEffect/"+nameof(DarkInTransitionEffectConfigurer), fileName = nameof(DarkInTransitionEffectConfigurer))]
    public class DarkInTransitionEffectConfigurer : BaseTransitionEffectConfigurer<DarkInTransitionEffect>
    {
        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}


