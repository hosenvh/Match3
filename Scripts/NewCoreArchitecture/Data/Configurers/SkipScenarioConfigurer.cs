using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using Match3.Game.SkipScenario;
using UnityEngine;


namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/Game/SkipScenario" + nameof(SkipScenarioConfigurer))]
    public class SkipScenarioConfigurer : ScriptableConfiguration, Configurer<SkipScenarioController>
    {
        public int requirePassedLevel;
        public int requireStackedStar;
        public int skipOfferInterval;
        
        public void Configure(SkipScenarioController entity)
        {
            entity.SetRequirePassedLevelIndex(requirePassedLevel - 1);
            entity.SetRequireStackedStar(requireStackedStar);
            entity.SetSkipOfferInterval(skipOfferInterval);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }

}