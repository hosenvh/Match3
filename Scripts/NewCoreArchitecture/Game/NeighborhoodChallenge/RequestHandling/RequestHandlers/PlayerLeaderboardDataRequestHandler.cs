using NiceJson;
using System;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class PlayerLeaderboardDataRequestHandler : GenericNCServerRequestHandler<PlayerLeaderboardDataResult>
    {

        public void Request(string challengeName, Action<PlayerLeaderboardDataResult> onSuccess, Action<NCFailureResult> onFailure)
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

        protected override PlayerLeaderboardDataResult ExtractSuccessResult(JsonNode body)
        {
            throw new NotImplementedException();
            //return new PlayerLeaderboardDataResult(0, 0);
        }
    }
}