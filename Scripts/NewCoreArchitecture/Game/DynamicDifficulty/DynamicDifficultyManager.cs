using System;
using System.Linq;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.EventManagement;
using Match3.Overlay.Analytics;


namespace Match3.Game.DynamicDifficulty
{
    public class DynamicDifficultyManager : Service
    {
        public readonly DynamicDifficultyApplier dynamicDifficultyApplier;

        public DynamicDifficultyManager(UserProfileManager profile, EventManager eventManager, ServerConfigData.DynamicDifficulty config)
        {
            DifficultyDetector detector = new DifficultyDetector(profile, eventManager, config.neededLooseCount);
            dynamicDifficultyApplier = new DynamicDifficultyApplier(detector, profile, eventManager,
                config.rewards.Select(reward => new DynamicDifficultyApplier.DynamicDifficultyReward(new TimeSpan(0, 0, 0, seconds: reward.neededPassedTimeSeconds),
                                                                                                     new TimeSpan(0, 0, 0, seconds: reward.infiniteLifeRewardDuration),
                                                                                                     reward.rainbowFactorCoefficient))
                                                                                                    .ToList()
                                    );

            dynamicDifficultyApplier.TryActivating();

            AnalyticsManager.analyticsAdaptersManager.RegisterAnalyticsHandler<GameplayAnalyticsAdapter>(new DynamicDifficultySpecificLevelEntriesAnalyticsHandler(dynamicDifficultyApplier));
        }
    }
}