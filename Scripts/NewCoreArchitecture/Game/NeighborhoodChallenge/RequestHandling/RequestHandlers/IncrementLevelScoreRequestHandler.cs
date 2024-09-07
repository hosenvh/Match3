using System;
using NiceJson;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{


    public class IncrementLevelScoreRequestHandler : GenericNCServerRequestHandler<IncrementLevelScoreResult>
    {
        public void Request(int score, string challengeName, Action<IncrementLevelScoreResult> onSuccess, Action<NCFailureResult> onFailure)
        {
            var body =
                CreateBodyObject().
                AddChallengeName(challengeName).
                AddEnvironment().
                AddPlayerId().
                AddMarket().
                AddPackageName().
                Add("type","SCORE").
                Add("score", score);

            var request = 
                CreateRequestBuilderFor("player/challenge/inc-score").
                SetBody(body).
                SetType(Network.HTTPRequestType.POST).
                Build();

            RequestFromServer(request, onSuccess, onFailure);
        }

        protected override IncrementLevelScoreResult ExtractSuccessResult(JsonNode body)
        {
            return new IncrementLevelScoreResult(int.Parse(body["score"]), int.Parse(body["position"]));
        }

        protected override NCFailureResult ExtractFailureResult(string error)
        {
            if (HasServerErrorCode(error, "player-banned-for-this-packageName"))
                return new PlayerBannedFailureResult();
            else if (HasServerErrorCode(error, "challenge-finished_exception"))
                return new ChallengeFinishedFailureResult();
            else
                return base.ExtractFailureResult(error);
        }
    }
}