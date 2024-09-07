using System;
using Match3.Game.ServerData;
using NiceJson;
using static Match3.Game.NeighborhoodChallenge.RequestHandling.ParsingUtilities;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class RewardClaimingRequestHandler : GenericNCServerRequestHandler<RewardClaimResult>
    {

        public void Request(string challengeName, Action<RewardClaimResult> onSuccess, Action<NCFailureResult> onFailure)
        {
            var body =
                CreateBodyObject().
                AddChallengeName(challengeName).
                AddEnvironment().
                AddPlayerId().
                AddMarket().
                AddPackageName();

            var request =
                CreateRequestBuilderFor("player/challenge/update/claim").
                SetBody(body).
                SetType(Network.HTTPRequestType.POST).
                Build();

            RequestFromServer(request, onSuccess, onFailure);
        }

        protected override RewardClaimResult ExtractSuccessResult(JsonNode body)
        {
            var serverRewardsData = UnityEngine.JsonUtility.FromJson<ServerRewardsData>(body.ToJsonString());

            return new RewardClaimResult(ParsingUtilities.ConvertToChallengeRankings(serverRewardsData.rankingRewards));
        }

        protected override NCFailureResult ExtractFailureResult(string error)
        {
            if (HasServerErrorCode(error, "player_claimed_before"))
            {
                var serverRewardsData = UnityEngine.JsonUtility.FromJson<ServerRewardsData>(NiceJson.JsonNode.ParseJsonString(error)["data"].ToJsonString());
                return new RewardAlreadyClaimedFailureResult(ParsingUtilities.ConvertToChallengeRankings(serverRewardsData.rankingRewards));
            }
            return base.ExtractFailureResult(error);
        }
    }
}