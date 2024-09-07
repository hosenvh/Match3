using Match3.Foundation.Base.Configuration;
using Match3.TransitionEffect.Data;
using UnityEngine;


namespace Match3.Presentation.TransitionEffects
{
    [CreateAssetMenu(menuName = "Match3/Configurations/TransitionEffect/"+nameof(CloudTransitionEffectConfigurer), fileName = nameof(CloudTransitionEffectConfigurer))]
    public class CloudTransitionEffectConfigurer : BaseTransitionEffectConfigurer<CloudTransitionEffect>
    {
        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }    
}


