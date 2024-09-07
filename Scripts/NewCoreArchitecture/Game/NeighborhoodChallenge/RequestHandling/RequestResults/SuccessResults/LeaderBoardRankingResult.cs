using System.Collections.Generic;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class LeaderBoardRankingResult : NCSuccessResult
    {
        public readonly List<LeaderboardRanking> rankings;

        public LeaderBoardRankingResult(List<LeaderboardRanking> rankings)
        {
            this.rankings = rankings;
        }
    }

    public class LeaderBoardResult : NCSuccessResult
    {
        public readonly List<LeaderboardRanking> rankings;
        public readonly int playerRank;
        public readonly int playerScore;
        public readonly int totalUsersInChallenge;
        public readonly int totalUserWithNonZeroScoreInChallenge;

        public LeaderBoardResult(
            List<LeaderboardRanking> rankings, 
            int playerRank, 
            int playerScore, 
            int totalUsersInChallenge, 
            int totalUserWithNonZeroScoreInChallenge)
        {
            this.rankings = rankings;
            this.playerRank = playerRank;
            this.playerScore = playerScore;
            this.totalUsersInChallenge = totalUsersInChallenge;
            this.totalUserWithNonZeroScoreInChallenge = totalUserWithNonZeroScoreInChallenge;
        }
    }
}