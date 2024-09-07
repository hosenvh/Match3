using System;
using System.Collections.Generic;
using NiceJson;
using UnityEngine;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class LeaderBoardRequestHandler : GenericNCServerRequestHandler<LeaderBoardResult>
    {
        public const int LEADERBOARD_LIMIT = 100;

        public void Request(string challengeName, Action<LeaderBoardResult> onSuccess, Action<NCFailureResult> onFailure)
        {
            var body = CreateBodyObject().
                AddEnvironment().
                AddMarket().
                AddPackageName().
                AddChallengeName(challengeName).
                AddPlayerId().
                Add("limit", LEADERBOARD_LIMIT.ToString());

            var request =
                CreateRequestBuilderFor("leader-board/score-with-player-data").
                SetBody(body).
                SetType(Network.HTTPRequestType.POST).
                Build();

            RequestFromServer(request, onSuccess, onFailure);
        }

        protected override LeaderBoardResult ExtractSuccessResult(JsonNode body)
        {
            List<LeaderboardRanking> leaderboardRankings = new List<LeaderboardRanking>();

            foreach (var lData in body["playerRankData"] as JsonArray)
                leaderboardRankings.Add(JsonUtility.FromJson<LeaderboardRanking>(lData.ToJsonString()));

            return new LeaderBoardResult(
                leaderboardRankings,
                body["position"],
                body["score"],
                body["playerParticipateCounter"], 
                body["playerParticipateWithNoZeroScoreCounter"]);
        }
    }
}