using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using Match3.Game.NeighborhoodChallenge;
using Match3.Network;
using UnityEngine;

namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/Gameplay/NC/" + nameof(NeighborhoodChallengeConfigurer))]
    public class NeighborhoodChallengeConfigurer : 
        ScriptableConfiguration, 
        Configurer<NeighborhoodChallengeActivationPolicy>,
        Configurer<NCLevelChangingController>,
        Configurer<NCTicket>

    {
        [System.Serializable]
        public struct TicketConfig
        {
            public float regenerationDurationInSeconds;
        }

        public int levelChangingCost;
        public bool isEnabled;

        public TaskConfig activationTaskConfig;
        public int activationLevelIndex;

        public TicketConfig ticketConfig;

        public void Configure(NeighborhoodChallengeActivationPolicy entity)
        {
            entity.SetIsEnabled(isEnabled);
            entity.SetActivationInfo(activationTaskConfig, activationLevelIndex);
        }

        public void Configure(NCTicket entity)
        {
            entity.SetRegenerationDurationInSeconds(ticketConfig.regenerationDurationInSeconds);
        }

        public void Configure(NCLevelChangingController entity)
        {
            entity.SetChangeCost(levelChangingCost);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register<NeighborhoodChallengeActivationPolicy>(this);
            manager.Register<NCLevelChangingController>(this);
            manager.Register<NCTicket>(this);
        }


    }
}