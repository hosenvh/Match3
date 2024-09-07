using System;
using System.Collections.Generic;
using NiceJson;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class PlayerRelevantChallengesRequestHandler : GenericNCServerRequestHandler<PlayerRelevantChallengesResult>
    {
        public void Request(Action<PlayerRelevantChallengesResult> onSuccess, Action<NCFailureResult> onFailure)
        {
            var request =
                CreateRequestBuilderFor("player/challenge/get-all").
                AddEnvironment().
                AddPlayerID().
                AddMarket().
                AddPackageName().
                SetType(Network.HTTPRequestType.GET).
                Build();

            RequestFromServer(request, onSuccess, onFailure);
        }

        protected override PlayerRelevantChallengesResult ExtractSuccessResult(JsonNode body)
        {
            return new PlayerRelevantChallengesResult(
                ExtractChallengNamesFrom(body["active"] as JsonArray), 
                ExtractChallengNamesFrom(body["notClaimed"] as JsonArray));
        }

        private List<string> ExtractChallengNamesFrom(JsonArray jsonArray)
        {
            var challengesNames = new List<string>();

            foreach (var element in jsonArray)
                challengesNames.Add(element["challengeName"]);

            return challengesNames;
        }
    }
}