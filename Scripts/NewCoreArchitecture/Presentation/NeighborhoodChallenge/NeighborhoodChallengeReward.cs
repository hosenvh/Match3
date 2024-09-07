using System;
using System.Collections.Generic;
using Match3.Game;

namespace Match3.Game.NeighborhoodChallenge
{

    [Serializable]
    public class ChallengeRankingReward
    {
        public List<Reward> rewards;
        public int start = 0;
        public int end = 0;

        public ChallengeRankingReward(List<Reward> rewards, int start, int end)
        {
            this.rewards = rewards;
            this.start = start;
            this.end = end;
        }

        public bool IsInRange(int userRank)
        {
            if (IsLastReward())
                return start <= userRank;
            return start <= userRank && userRank <= end;
        }

        public bool IsLastReward()
        {
            return end == -1;
        }

        public void Apply()
        {
            foreach (var reward in rewards)
                reward.Apply();
        }
    }

}