using Match3.Game.NeighborhoodChallenge.RequestHandling;
using System;

namespace Match3.Game.NeighborhoodChallenge
{
    public interface LevelScoringPort : NeighborhoodChallengePort
    {
        void OnScoreSubmited();
    }

    public class NCLevelScoringController : NeighborhoodChallengeController
    {
        public NCLevelScoringController(NeighborhoodChallengeManager manager, GameTransitionManager transitionManager) : base(manager, transitionManager)
        {
        }

        public void SubmitLevelScore(int score)
        {
            var handler = new IncrementLevelScoreRequestHandler();
            var port = manager.GetPort<LevelScoringPort>();
            port.Wait();
            handler.Request(
                score,
                manager.UserInfo().CurrentChallenge().name,
                onSuccess: (r) =>
                {
                    manager.UserInfo().SetScore(r.totalScore);
                    manager.UserInfo().SetRank(r.rank);
                    port.OnScoreSubmited();
                },
                onFailure: (e) => HandleIncrementScoreFailure(e, port)
                );
        }

        void HandleIncrementScoreFailure(NCFailureResult failureResult, LevelScoringPort port)
        {
            if (failureResult is PlayerBannedFailureResult)
                manager.UserInfo().IsBanned = true;

            port.HandleError(AddMapExitTo(failureResult));
        }
    }
}