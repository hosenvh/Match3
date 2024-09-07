using Match3.Presentation.Gameplay;
using static Base;


namespace Match3.Overlay.Analytics.LevelEntries
{
    public class ChallengeLevelSpecificLevelEntriesAnalyticsHandler : SpecificLevelEntriesAnalyticsHandler
    {
        protected override string GetSpecificCategoryName()
        {
            return "ChallengeLevel";
        }

        protected override bool ShouldSendEvent()
        {
            return gameManager.CurrentState is CampaignGameplayState campaignGameplayState && campaignGameplayState.IsChallengeLevel;
        }
    }
}