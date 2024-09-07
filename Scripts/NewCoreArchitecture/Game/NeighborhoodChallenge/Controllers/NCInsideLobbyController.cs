using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NeighborhoodChallenge.RequestHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Utility.GolmoradLogging;
using UnityEngine;

namespace Match3.Game.NeighborhoodChallenge
{
    public interface LobbyControllerPort : NeighborhoodChallengePort
    {
        void ShowRewardClaimingFor(ChallengeRankingReward reward, LeaderBoardData leaderBoardData, NCUserInfo userInfo, Action onCompleted);

        void ShowCurrentLeaderBoard(LeaderBoardData leaderBoardData, NCUserInfo userInfo);
    }

    public class NCInsideLobbyController : NeighborhoodChallengeController
    {
        LeaderBoardRequestHandler leaderboardRequestHandler = new LeaderBoardRequestHandler();
        RewardClaimingRequestHandler rewardClaimingRequestHandler = new RewardClaimingRequestHandler();

        LeaderBoardDataCache currentLeaderBoardDataCache = new LeaderBoardDataCache();

        UserLeaderBoardRankFixer userLeaderBoardRankFixer;

        public NCInsideLobbyController(NeighborhoodChallengeManager manager, GameTransitionManager transitionManager) : base(manager, transitionManager)
        {
            userLeaderBoardRankFixer = new UserLeaderBoardRankFixer(ServiceLocator.Find<IUserProfile>());
        }

        public void SetupLoby()
        {
            var challengeName = manager.UserInfo().CurrentChallenge().name;
            var port = manager.GetPort<LobbyControllerPort>();

            if (manager.UserHasNotClaimedReward())
                RequestClaimReward(manager.NotClaimedChallengeName(), port);
            else
                RequestLeaderBoardDataForCurrentChallenge(port, delegate { }) ;   
        }

        private void RequestClaimReward(string challengeName, LobbyControllerPort port)
        {
            port.Wait();

            Action<List<LeaderboardRanking>> onPreviousLeaderBoardSuccess = (rankings) =>
            {
                var challengeData = new ChallengeData();
                challengeData.name = challengeName;
                var leaderBoardData = new LeaderBoardData(rankings, challengeData);

                rewardClaimingRequestHandler.Request(
                challengeName,
                onSuccess: (r) => ApplyRewardForChallenge(leaderBoardData, r.rankingRewards, port),
                onFailure: (e) => HandleClaimError(e, leaderBoardData, port));
            };

            RequestLeaderBoardDataForPreviousChallenge(
                challengeName,
                port,
                onCompleted: onPreviousLeaderBoardSuccess);
        }

        private void HandleClaimError(NCFailureResult error, LeaderBoardData leaderboardData, LobbyControllerPort port)
        {
            if (error is RewardAlreadyClaimedFailureResult alreadyClaimedError)
                ApplyRewardForChallenge(leaderboardData, alreadyClaimedError.rankingRewards, port);
            else
                port.HandleError(AddMapExitTo(error));
        }

        private void ApplyRewardForChallenge(LeaderBoardData leaderBoardData, List<ChallengeRankingReward> rankingRewards, LobbyControllerPort port)
        {
            leaderBoardData.challengeData.rankingRewards = rankingRewards;
            var userReward = FindUserRewardIn(rankingRewards);

            userReward.Apply();

            manager.MarkAsClaimed(leaderBoardData.challengeData.name);

            port.ShowRewardClaimingFor(userReward, leaderBoardData, manager.UserInfo(), () => RequestLeaderBoardDataForCurrentChallenge(port, delegate { }));
        }

        private void RequestLeaderBoardDataForCurrentChallenge(LobbyControllerPort port, Action onCompleted)
        {
            port.Wait();

            if (currentLeaderBoardDataCache.IsValid())
            {
                UpdateCachedLeaderBoardBasedOnPlayerScore(
                    currentLeaderBoardDataCache.GetData(),
                    manager.UserInfo(),
                    ServiceLocator.Find<IUserProfile>().GlobalUserId);
                port.ShowCurrentLeaderBoard(currentLeaderBoardDataCache.GetData(), manager.UserInfo());
                onCompleted();
            }
            else
            {
                GetLeaderBoardData(
                       manager.UserInfo().CurrentChallenge().name,
                       port,
                       onCompleted: (rankings) =>
                       {
                           var leaderData = new LeaderBoardData(rankings, manager.UserInfo().CurrentChallenge());
                           currentLeaderBoardDataCache.SetData(leaderData);
                           port.ShowCurrentLeaderBoard(leaderData, manager.UserInfo());
                           onCompleted();
                       });
            }
        }

        private void RequestLeaderBoardDataForPreviousChallenge(string challengeName, LobbyControllerPort port, Action<List<LeaderboardRanking>> onCompleted)
        {
            GetLeaderBoardData(challengeName, port, onCompleted);
        }

        private void GetLeaderBoardData(string challengeName, LobbyControllerPort port, Action<List<LeaderboardRanking>> onCompleted)
        {
            Action<LeaderBoardResult> leaderboardSuccessAction = result =>
            {
                manager.UserInfo().SetRank(result.playerRank);
                manager.UserInfo().SetScore(result.playerScore);
                userLeaderBoardRankFixer.FixUserRank(
                    manager.UserInfo(), 
                    result.rankings, 
                    result.totalUsersInChallenge, 
                    result.totalUserWithNonZeroScoreInChallenge);
                onCompleted(result.rankings);
            };

            leaderboardRequestHandler.Request(challengeName, leaderboardSuccessAction, (e) => port.HandleError(AddMapExitTo(e)));
        }



        // NOTE: This is public static for testing.
        // NOTE: This means the class is not testable.
        // NOTE: This can be much more optimized.
        // TODO: Refactor this.
        public static void UpdateCachedLeaderBoardBasedOnPlayerScore(LeaderBoardData leaderBoardData, NCUserInfo userInfo, string userId)
        {
            var rankings = leaderBoardData.rankings;


            if (userInfo.Score <= leaderBoardData.rankings.Last().score)
                return;

            var startLength = rankings.Count;

            rankings.RemoveAll(p => p.globalUniqueId.Equals(userId));

            var insertionIndex = rankings.FindIndex(0, r => r.score < userInfo.Score);
            if (insertionIndex == -1)
                insertionIndex = 0;

            var newRank = new LeaderboardRanking() { globalUniqueId = userId, score = userInfo.Score, username = userInfo.UserName };
            userInfo.SetRank(insertionIndex + 1);
            rankings.Insert(insertionIndex, newRank);

            for (int i = 0; i < rankings.Count; ++i)
                rankings[i].rank = i + 1;
            
            rankings.RemoveRange(startLength, rankings.Count - startLength);

        }


        private ChallengeRankingReward FindUserRewardIn(List<ChallengeRankingReward> rankingRewards)
        {
            var userRank = manager.UserInfo().Rank;

            // NOTE: A TEMPORARY SOLUTION FOR FIXING -1 RANK.
            if(userRank == -1)
            {
                userRank = 2002;
                DebugPro.LogError<NeighborHoodChallengeLogTag>("User rank was -1");
            }


            foreach (var rankingReward in rankingRewards)
                if (rankingReward.IsInRange(userRank))
                    return rankingReward;

            throw new Exception($"Could not find reward for rank {userRank}");
        }

    }
}