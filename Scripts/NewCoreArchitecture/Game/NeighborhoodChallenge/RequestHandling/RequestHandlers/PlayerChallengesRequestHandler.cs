using System;
using System.Collections.Generic;
using NiceJson;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{

    public class PlayerChallengesRequestHandler : GenericNCServerRequestHandler<PlayerChallengesResult>
    {
        public void Request(bool isActive, bool isClaimed, Action<PlayerChallengesResult> onSuccess, Action<NCFailureResult> onFailure)
        {
            var request = 
                CreateRequestBuilderFor("player/challenge/get-all").
                AddEnvironment().
                AddPlayerID().
                AddMarket().
                AddPackageName().
                AddParameters("isActive", isActive.ToString()).
                AddParameters("isClaimed", isClaimed.ToString()).
                SetType(Network.HTTPRequestType.GET).
                Build();

            RequestFromServer(request, onSuccess, onFailure);
        }

        protected override PlayerChallengesResult ExtractSuccessResult(JsonNode body)
        {
            var challengesNames = new List<string>();

            foreach (var element in body as NiceJson.JsonArray)
                challengesNames.Add(element["challengeName"]);

            return new PlayerChallengesResult(challengesNames);
        }
    }
}