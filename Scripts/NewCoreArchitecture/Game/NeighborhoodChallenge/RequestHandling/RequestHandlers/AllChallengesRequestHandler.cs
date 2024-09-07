using NiceJson;
using System;
using System.Collections.Generic;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class AllChallengesRequestHandler : GenericNCServerRequestHandler<AllChallengesResult>
    {
        public void Request(bool activeChallengesOnly, Action<AllChallengesResult> onSuccess, Action<NCFailureResult> onFailure)
        {
            var request =
                CreateRequestBuilderFor("challenge/get-all").
                AddPackageName().
                AddParameters("isActive", activeChallengesOnly.ToString()).
                SetType(Network.HTTPRequestType.GET).
                Build();

            RequestFromServer(request, onSuccess, onFailure);
        }

        protected override AllChallengesResult ExtractSuccessResult(JsonNode body)
        {
            return new AllChallengesResult(ParsingUtilities.ParseChallengesData(body as JsonArray));
        }
    }
}