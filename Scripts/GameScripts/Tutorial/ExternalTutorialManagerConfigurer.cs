using System.Collections.Generic;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using Match3.Game.Tutorial;


namespace Match3.Data.Configuration
{
    public abstract class ExternalTutorialManagerConfigurer <T> : ScriptableConfiguration, Configurer<T> where T : ExternalTutorialManager
    {
        public List<int> tutorialSequence;
        public float startDelay;
        public float stepDelay;
        
        public void Configure(T entity)
        {
            entity.SetTutorialSequence(tutorialSequence);
            entity.SetDelays(startDelay, stepDelay);
        }
        
        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(configurer: this);
        }
    }
}


