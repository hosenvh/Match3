using Match3.Game;
using Match3.Game.ShopManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Data.ShopManagement
{
    [Serializable]
    public class PackageRewardsModifier
    {
        [SerializeField] MultiRewardHardCurrencyPackage.RewardList rewards = new MultiRewardHardCurrencyPackage.RewardList(new List<Reward>());

        public void Apply(MultiRewardHardCurrencyPackage package)
        {
            if (rewards == null || rewards.Collection() == null || rewards.Collection().Count == 0)
                return;

            package.OverrideRewards(rewards);
        }
    }

}