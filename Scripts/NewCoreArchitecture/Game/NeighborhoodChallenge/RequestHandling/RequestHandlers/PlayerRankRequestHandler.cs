using NiceJson;
using System;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{

    public class PlayerRankRequestHandler : GenericNCServerRequestHandler<PlayerRankResult>
    {

        public void Request(string challengeName, Action<PlayerRankResult> onSuccess, Action<NCFailureResult> onFailure)
        {
            var request = CreateRequestBuilderFor("leader-board/get-player-position").
                AddEnvironment().
                AddMarket().
                AddPackageName().
                AddPlayerID().
                AddParameters("challengeName", challengeName).
                SetType(Network.HTTPRequestType.GET).
                Build();

            RequestFromServer(request, onSuccess, onFailure);
        }

        protected override PlayerRankResult ExtractSuccessResult(JsonNode body)
        {
            return new PlayerRankResult(body);
        }
    }
}