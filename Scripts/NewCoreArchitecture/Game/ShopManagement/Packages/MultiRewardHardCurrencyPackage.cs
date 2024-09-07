using Match3.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Match3.Game.ShopManagement
{
    public class MultiRewardHardCurrencyPackage : HardCurrencyPackage
    {
        [Serializable]
        public class RewardList : PolymorphicSerializableCollection<List<Reward>, Reward>
        {
            public RewardList(List<Reward> collection) : base(collection)
            {
            }
        }

        /*  --------- Serialization Changelog -----------
         *  Date: 2021/10/25
         *  Affected Version: TBA
         *  Formerly Serialized As: RewardList rewards
         *  Reason: New reward types are added to the game which are not supported in previous versions  
         */
        [FormerlySerializedAs("rewards")]
        [SerializeField] RewardList rewards_v2 = new RewardList(new List<Reward>());

        public override void Apply()
        {
            foreach (var reward in rewards_v2.Collection())
                reward.Apply();
        }

        public List<Reward> Rewards()
        {
            return rewards_v2.Collection();
        }

        public void OverrideRewards(RewardList rewards)
        {
            this.rewards_v2 = rewards;
        }
    }
}