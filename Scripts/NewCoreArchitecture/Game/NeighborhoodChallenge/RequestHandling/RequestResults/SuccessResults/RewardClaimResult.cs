using System.Collections.Generic;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class RewardClaimResult : NCSuccessResult
    {
        public List<ChallengeRankingReward> rankingRewards;

        public RewardClaimResult(List<ChallengeRankingReward> rankingRewards)
        {
            this.rankingRewards = rankingRewards;
        }
    }
}