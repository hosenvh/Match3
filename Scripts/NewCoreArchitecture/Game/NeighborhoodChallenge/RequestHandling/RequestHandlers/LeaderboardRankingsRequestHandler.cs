using System;
using System.Collections.Generic;
using NiceJson;
using UnityEngine;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{

    public class LeaderboardRankingsRequestHandler : GenericNCServerRequestHandler<LeaderBoardRankingResult>
    {
        public const int LEADERBOARD_LIMIT = 100;

        public void Request(string challengeName, Action<LeaderBoardRankingResult> onSuccess, Action<NCFailureResult> onFailure)
        {
            var body = CreateBodyObject().
                AddEnvironment().
                AddMarket().
                AddPackageName().
                AddChallengeName(challengeName).
                Add("limit", LEADERBOARD_LIMIT.ToString());

            var request =
                CreateRequestBuilderFor("leader-board/score").
                SetBody(body).
                SetType(Network.HTTPRequestType.POST).
                Build();

            RequestFromServer(request, onSuccess, onFailure);
        }

        protected override LeaderBoardRankingResult ExtractSuccessResult(JsonNode body)
        {
            List<LeaderboardRanking> leaderboardRankings = new List<LeaderboardRanking>();

            foreach (var lData in body as JsonArray)
                leaderboardRankings.Add(JsonUtility.FromJson<LeaderboardRanking>(lData.ToJsonString()));

            return new LeaderBoardRankingResult(leaderboardRankings);
        }
    }
}