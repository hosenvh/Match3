using Match3.Development.Base;
using Match3.Development.Base.DevelopmentConsole;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.ReferralMarketing;
using Match3.Presentation.Rewards.Components;
using static Base;


namespace Match3.Development.DevOptions
{
    [DevOptionGroup(groupName: "Referral", priority: 15)]
    public class ReferralDevOptions : DevelopmentOptionsDefinition
    {
        [DevOption(commandName: "Play Cat Scenario", shouldAutoClose: true)]
        public static void PlayCatScenario()
        {
            PlayReferralScenario(3);
        }

        [DevOption(commandName: "Play Lamborghini Scenario", shouldAutoClose: true)]
        public static void PlayLamborghiniScenario()
        {
            PlayReferralScenario(5);
        }

        private static void PlayReferralScenario(int scenarioGoalId)
        {
            var referralCenter = ServiceLocator.Find<ReferralCenter>();
            var scenarioConfig = FindScenarioConfig();
            gameManager
               .mapManager
               .CurrentMap
               .mapStateScenarioPlayer
               .PlayScenario(scenarioConfig, onFinish: () => {  });

            ScenarioConfig FindScenarioConfig()
            {
                foreach (var reward in referralCenter.GetGoalReferralReward(scenarioGoalId).rewards)
                {
                    if (reward.GetComponent<RewardPresentationClaimScenarioComponent>() != null)
                    {
                        var claimScenarioComponent = reward.GetComponent<RewardPresentationClaimScenarioComponent>();
                        return claimScenarioComponent.ResourceScenarioConfigAsset.Load();
                    }
                }

                return null;
            }
        }
    }
}