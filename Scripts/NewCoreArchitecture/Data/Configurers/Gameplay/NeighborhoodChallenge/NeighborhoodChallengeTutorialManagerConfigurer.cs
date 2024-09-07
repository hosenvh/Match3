using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using Match3.Game.NeighborhoodChallenge;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/Gameplay/NC/NeighborhoodChallengeTutorialManager")]
    public class NeighborhoodChallengeTutorialManagerConfigurer : ScriptableConfiguration, Configurer<NeighborhoodChallengeTutorialManager>
    {
        public List<int> tutorialSequence;
        public float startDelay;
        public float stepDelay;

        public void Configure(NeighborhoodChallengeTutorialManager entity)
        {
            entity.SetTutorialSequence(tutorialSequence);
            entity.SetDelays(startDelay, stepDelay);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}