using System;
using System.Collections.Generic;
using Match3.Game;
using Match3.Profiler;
using UnityEngine;

namespace Match3.LevelInfoAds.RewardHandler
{
    public abstract class LevelInfoAdsRewardHandler
    {
        public Reward SelectARandomReward()
        {
            List<Type> rewards = GetPossibleRewardsTypes();
            Type rewardType = rewards.RandomOne();
            return (Reward) Activator.CreateInstance(rewardType, 1);
        }

        public void GiveReward(Reward reward)
        {
            GetCorrespondingLevelReservedRewardHandler().AddReservedReward(reward.GetType());
        }

        protected abstract List<Type> GetPossibleRewardsTypes();
        protected abstract LevelReservedRewardsHandler GetCorrespondingLevelReservedRewardHandler();

        public abstract string GetRewardConfirmPopupMessage();
    }
}