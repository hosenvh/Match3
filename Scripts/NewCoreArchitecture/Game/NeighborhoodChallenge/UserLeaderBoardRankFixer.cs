using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Match3.Game.NeighborhoodChallenge
{
    // The player's rank given in rankings may not always be the same as the player's rank given from server. 
    // If the player is in the leader rankings, this updates player's rank to match the leaderboard.
    // Also in some situations the server rank may be -1.
    public class UserLeaderBoardRankFixer
    {
        private struct FixingInformation
        {
            public readonly NCUserInfo userInfo;
            public readonly List<LeaderboardRanking> rankings;
            public readonly int totalUsers;
            public readonly int totalNonZeroScoreUsers;

            public FixingInformation(NCUserInfo userInfo, List<LeaderboardRanking> rankings, int totalUsers, int totalNonZeroScoreUsers)
            {
                this.userInfo = userInfo;
                this.rankings = rankings;
                this.totalUsers = totalUsers;
                this.totalNonZeroScoreUsers = totalNonZeroScoreUsers;
            }
        }

        const float ESTIMATION_FORMULA_EXPONENT_COEFFICIENT = 7.5f;
        const float ESTIMATION_FORMULA_OFFSET = -15;


        IUserProfile userProfile;

        public UserLeaderBoardRankFixer(IUserProfile userProfile)
        {
            this.userProfile = userProfile;
        }

        public void FixUserRank(NCUserInfo userInfo, List<LeaderboardRanking> rankings, int totalUsers, int totalNonZeroScoreUsers)
        {
            // TODO: Remove Try/Catch clause if everything is proven to be safe.
            try
            {
                var info = new FixingInformation(userInfo, rankings, totalUsers, totalNonZeroScoreUsers);

                // NOTE: The order of this calls are very important
                SetRankToRankingsRank(
                    info,
                    onNotFixed: () => StopIfRankIsPositive(
                        info,
                        onNotFixed: () => SetRankToLastWhenScoreIsZero(
                            info,
                            onNotFixed: () => EstimateRankBasedOnScore(info))));
            }
            catch(Exception e)
            {
                Debug.LogError($"Exception in fixing user's rank:\n{e.Message}\n{e.StackTrace}");
                userInfo.SetRank(2002);
            }
        }

        private void SetRankToRankingsRank(FixingInformation info, Action onNotFixed)
        {
            var length = info.rankings.Count;
            var userGlobalUniqueId = userProfile.GlobalUserId;

            for (int i = 0; i < length; ++i)
            {
                if (userGlobalUniqueId.Equals(info.rankings[i].globalUniqueId))
                {
                    info.userInfo.SetRank(i + 1);
                    return;
                }
            }

            onNotFixed.Invoke();
        }

        private void StopIfRankIsPositive(FixingInformation info, Action onNotFixed)
        {
            if (info.userInfo.Rank > 0)
                return;

            onNotFixed.Invoke();
        }

        private void SetRankToLastWhenScoreIsZero(FixingInformation info, Action onNotFixed)
        {
            if (info.userInfo.Score <= 0)
                info.userInfo.SetRank(info.totalNonZeroScoreUsers + 1);
            else
                onNotFixed.Invoke();
        }


        private void EstimateRankBasedOnScore(FixingInformation info)
        {
            var estimation = CalculateEstimationFormula(
                currentScore: info.userInfo.Score, 
                highestScore: info.rankings.Last().score,
                rankOffset: info.rankings.Count + 1, 
                totalNonZeroScoreUsers: info.totalNonZeroScoreUsers - info.rankings.Count);

            estimation = Mathf.Clamp(estimation, info.rankings.Count + 1, info.totalNonZeroScoreUsers);

            info.userInfo.SetRank(estimation);

        }


        private int CalculateEstimationFormula(int currentScore, int highestScore, int rankOffset, int totalNonZeroScoreUsers)
        {
            var estimation = (totalNonZeroScoreUsers * Math.Exp((-1f / highestScore) * ESTIMATION_FORMULA_EXPONENT_COEFFICIENT * currentScore)) + rankOffset + ESTIMATION_FORMULA_OFFSET;
            return (int) Math.Ceiling(estimation);
        }
    }
}