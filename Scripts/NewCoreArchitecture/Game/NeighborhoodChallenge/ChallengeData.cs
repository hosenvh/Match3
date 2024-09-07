using System;
using System.Collections.Generic;

namespace Match3.Game.NeighborhoodChallenge
{
    public class ChallengeData
    {
        public string name;
        public DateTime startTime;
        public DateTime endTime;
        public List<ChallengeRankingReward> rankingRewards = new List<ChallengeRankingReward>();
    }
}