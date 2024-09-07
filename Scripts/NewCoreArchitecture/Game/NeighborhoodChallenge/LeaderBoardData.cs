using System;
using System.Collections.Generic;

namespace Match3.Game.NeighborhoodChallenge
{
    [Serializable]
    public class LeaderBoardData
    {
        public readonly List<LeaderboardRanking> rankings;
        public readonly ChallengeData challengeData;

        public LeaderBoardData(List<LeaderboardRanking> rankings, ChallengeData challengeData)
        {
            this.rankings = rankings;
            this.challengeData = challengeData;
        }
    }

    [Serializable]
    public class LeaderboardRanking
    {
        public string globalUniqueId;
        public string username;
        public int rank;
        public int score;
    }
}