using Match3.Data;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using Match3.Game;
using UnityEngine;


namespace Match3.Development.DevOptions.Configurers
{
    [CreateAssetMenu(menuName = "Match3/Development/RewardDevOptionsConfigurer", fileName = nameof(RewardDevOptionsConfigurer))]
    public class RewardDevOptionsConfigurer : ScriptableConfiguration, Configurer<RewardsDevOptions>
    {
        [SerializeField] private SelectableRewardBundle rewardBundle;

        public Reward Reward => rewardBundle.GetReward();

        public void Configure(RewardsDevOptions entity)
        {
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}