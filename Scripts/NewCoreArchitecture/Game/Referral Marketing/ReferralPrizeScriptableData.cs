using System;
using System.Collections.Generic;
using I2.Loc;
using Match3.Data;
using Match3.Foundation.Unity;
using Match3.Game;
using Match3.Game.ReferralMarketing;
using PandasCanPlay.HexaWord.Utility;
using UnityEngine;


namespace Match3.Data.ReferralMarketing
{
    
    [CreateAssetMenu(menuName = "Match3/ReferralCenter/ReferralRewardData")]
    public class ReferralPrizeScriptableData : ScriptableObject
    {
        [Space(10)] 
        public int goalId;
        public int referredPlayerRequire;
        
        public List<SelectableReward> rewards;

        [Space(10)]
        public ResourceSpriteAsset rewardIcon;
        
        [Space(10)]
        public LocalizedStringTerm description;

        private ReferralReward referralReward  = null;


        public Sprite GetRewardIcon()
        {
            return rewardIcon.Load();
        }

        public ReferralReward GetReferralReward()
        {
            if (referralReward != null && referralReward.rewards!=null)
                return referralReward;
            
            ReferralReward refReward = new ReferralReward();
            List<Reward> rewardsList = new List<Reward>();

            foreach (var reward in rewards)
            {
                rewardsList.Add(reward.GetReward());
            }

            refReward.rewards = rewardsList.ToArray();

            referralReward = refReward;
            return refReward;
        }

        public void UpdateRewardsCount(ReferralReward refReward)
        {
            if (referralReward == null || referralReward.rewards==null) 
                GetReferralReward();

            foreach (var newReward in refReward.rewards)
            {
                foreach (var oldReward in referralReward.rewards)
                {
                    if (newReward.GetType() == oldReward.GetType())
                        oldReward.count = newReward.count;
                }
            }
        }
        
        
    }


}

