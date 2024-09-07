using NiceJson;
using System;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{

    public class ChallengeDataRequestHandler : GenericNCServerRequestHandler<ChallengeDataResult>
    {
        public void Request(string challengeName, Action<ChallengeDataResult> onSuccess, Action<NCFailureResult> onFailure)
        {
            var request =
                CreateRequestBuilderFor("challenge/get").
                AddPackageName().
                AddChallengeName(challengeName).
                SetType(Network.HTTPRequestType.GET).
                Build();

            RequestFromServer(request, onSuccess, onFailure);
        }

        protected override ChallengeDataResult ExtractSuccessResult(JsonNode body)
        {
            return new ChallengeDataResult(ParsingUtilities.ParseChallengeData(body));
        }

    }
}