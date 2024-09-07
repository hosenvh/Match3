using Match3.Game.NeighborhoodChallenge;
using Match3.Game.NeighborhoodChallenge.RequestHandling;
using System;
using I2.Loc;

namespace Match3.Presentation.NeighborhoodChallenge
{
    public class LevelScoringPortImp : NCPresentationPort, LevelScoringPort
    {
        Popup_NeighborhoodChallengeWin neighborhoodChallengeWinPopup;

        public LevelScoringPortImp(NeighborhoodChallengeManager challengeManager, Popup_NeighborhoodChallengeWin neighborhoodChallengeWinPopup) : base(challengeManager)
        {
            this.neighborhoodChallengeWinPopup = neighborhoodChallengeWinPopup;
        }


        public void Wait()
        {
            //BeginWaiting();
        }

        public void OnScoreSubmited()
        {
            EndWaiting();
            neighborhoodChallengeWinPopup.OnScoreSubmitted();
            neighborhoodChallengeWinPopup = null;
        }

        public void HandleError(NCFailureResult failureResult)
        {
            if (failureResult is RetriableFailureResult == false)
                neighborhoodChallengeWinPopup = null;

            if (failureResult is ChallengeFinishedFailureResult finishedResult)
            {
                waitingScreenController.CloseWaiting();
                Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_NeighborhoodChallenge.ChallengeIsFinished, ScriptLocalization.UI_General.Ok, "", true, (r) => finishedResult.confirmAction.Invoke());
            }
            else
                HandleGeneralError(failureResult);
        }

    }
}