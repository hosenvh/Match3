using System.Collections.Generic;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{

    public class RewardAlreadyClaimedFailureResult : BaseFailureResult
    {
        public List<ChallengeRankingReward> rankingRewards;

        public RewardAlreadyClaimedFailureResult(List<ChallengeRankingReward> rankingRewards)
        {
            this.rankingRewards = rankingRewards;
        }
    }
}