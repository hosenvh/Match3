using NiceJson;
using System;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{

    public class ChallengeRegisteringRequestHandler : GenericNCServerRequestHandler<ChallengeRegisteringResult>
    {
        public void Request(string challengeName, Action<ChallengeRegisteringResult> onSuccess, Action<NCFailureResult> onFailure)
        {
            var body = CreateBodyObject().
                 AddEnvironment().
                 AddMarket().
                 AddPackageName().
                 AddPlayerId().
                 AddChallengeName(challengeName);

            var request =
                CreateRequestBuilderFor("player/challenge/add").
                SetBody(body).
                SetType(Network.HTTPRequestType.POST).
                Build();

            RequestFromServer(request, onSuccess, onFailure);
        }

        protected override ChallengeRegisteringResult ExtractSuccessResult(JsonNode body)
        {
            return new ChallengeRegisteringResult(body["challengeName"]);
        }
    }
}