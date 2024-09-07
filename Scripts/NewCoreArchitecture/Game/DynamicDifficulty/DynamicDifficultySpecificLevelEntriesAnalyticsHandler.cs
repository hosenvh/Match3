using Match3.Overlay.Analytics.LevelEntries;
using Match3.Presentation.Gameplay;

namespace Match3.Game.DynamicDifficulty
{
    public class DynamicDifficultySpecificLevelEntriesAnalyticsHandler : SpecificLevelEntriesAnalyticsHandler
    {
        private readonly DynamicDifficultyApplier dynamicDifficultyApplier;

        public DynamicDifficultySpecificLevelEntriesAnalyticsHandler(DynamicDifficultyApplier dynamicDifficultyApplier)
        {
            this.dynamicDifficultyApplier = dynamicDifficultyApplier;
        }

        protected override bool ShouldSendEvent()
        {
            return Base.gameManager.CurrentState is CampaignGameplayState &&
                   dynamicDifficultyApplier.EffectiveRainbowAddValueFactor > 1;
        }

        protected override string GetSpecificCategoryName()
        {
            return "DynamicDifficulty";
        }
    }
}