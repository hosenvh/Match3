using System;
using NiceJson;


namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{

    public class PlayerScoreRequestHandler : GenericNCServerRequestHandler<PlayerScoreResult>
    {

        public void Request(string challengeName, Action<PlayerScoreResult> onSuccess, Action<NCFailureResult> onFailure)
        {
            var request = CreateRequestBuilderFor("player/challenge/get").
                AddEnvironment().
                AddPlayerID().
                AddMarket().
                AddPackageName().
                AddChallengeName(challengeName).
                SetType(Network.HTTPRequestType.GET).
                Build();

            RequestFromServer(request, onSuccess, onFailure);
        }

        protected override PlayerScoreResult ExtractSuccessResult(JsonNode body)
        {
            return new PlayerScoreResult(body["score"]);
        }
    }
}