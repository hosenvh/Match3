using System;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Presentation.Gameplay;


namespace Match3.Game.DynamicDifficulty
{
    public class DynamicDifficultyApplier : EventListener
    {
        public class DynamicDifficultyReward
        {
            public readonly TimeSpan neededPassedTimeSeconds;
            public readonly TimeSpan infiniteLifeRewardDuration;
            public readonly float rainbowFactorCoefficient;

            public DynamicDifficultyReward(TimeSpan neededPassedTimeSeconds, TimeSpan infiniteLifeRewardDuration, float rainbowFactorCoefficient)
            {
                this.neededPassedTimeSeconds = neededPassedTimeSeconds;
                this.infiniteLifeRewardDuration = infiniteLifeRewardDuration;
                this.rainbowFactorCoefficient = rainbowFactorCoefficient;
            }
        }

        private const string DYNAMIC_DIFFICULTY_RAINBOW_ADD_VALUE_FACTOR_KEY = "DynamicDifficulty_RainBowAddValueFactor";

        private readonly DifficultyDetector difficultyDetector;
        private readonly UserProfileManager profile;
        private readonly List<DynamicDifficultyReward> rewards;
        private TimeSpan remainedInfiniteLifeToGiveDuration; // This may be added to a Blackboard Model in future.

        public float EffectiveRainbowAddValueFactor
        {
            get => Base.gameManager.CurrentState is CampaignGameplayState ? profile.LoadData(DYNAMIC_DIFFICULTY_RAINBOW_ADD_VALUE_FACTOR_KEY, 1f) : 1;
            private set => profile.SaveData(DYNAMIC_DIFFICULTY_RAINBOW_ADD_VALUE_FACTOR_KEY, value);
        }

        public DynamicDifficultyApplier(DifficultyDetector difficultyDetector, UserProfileManager profile, EventManager eventManager, List<DynamicDifficultyReward> rewards)
        {
            this.profile = profile;
            this.difficultyDetector = difficultyDetector;
            this.rewards = rewards;
            eventManager.Register(this);
        }

        public bool IsInfiniteLifeRewardAvailableToGive()
        {
            return remainedInfiniteLifeToGiveDuration.Ticks > 0;
        }

        public TimeSpan GetAndConsumeRemainedInfiniteLifeToGiveDuration()
        {
            TimeSpan result = remainedInfiniteLifeToGiveDuration;
            remainedInfiniteLifeToGiveDuration = TimeSpan.Zero;
            return result;
        }

        public void TryActivating()
        {
            DynamicDifficultyReward selectedReward = TryToSelectRewardBasedOnPassedTimeFromDetection();
            if (selectedReward != null)
            {
                ApplyReward(reward: selectedReward);
                difficultyDetector.Restart();
            }
        }

        private DynamicDifficultyReward TryToSelectRewardBasedOnPassedTimeFromDetection()
        {
            DynamicDifficultyReward selectedReward = null;
            foreach (var reward in rewards)
            {
                if (difficultyDetector.GetPassedTimeFromDetection().TotalSeconds >= reward.neededPassedTimeSeconds.TotalSeconds
                 && (selectedReward == null || reward.neededPassedTimeSeconds > selectedReward.neededPassedTimeSeconds))
                    selectedReward = reward;
            }
            return selectedReward;
        }

        private void ApplyReward(DynamicDifficultyReward reward)
        {
            remainedInfiniteLifeToGiveDuration += reward.infiniteLifeRewardDuration;
            EffectiveRainbowAddValueFactor = reward.rainbowFactorCoefficient;
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            if (evt is LevelEndedEvent levelEndedEvent && levelEndedEvent.result == LevelResult.Win && Base.gameManager.CurrentState is CampaignGameplayState)
                EffectiveRainbowAddValueFactor = 1;
        }
    }
}