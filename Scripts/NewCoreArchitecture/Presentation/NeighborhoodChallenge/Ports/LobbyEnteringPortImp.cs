

using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.NeighborhoodChallenge;
using Match3.Game.NeighborhoodChallenge.RequestHandling;

namespace Match3.Presentation.NeighborhoodChallenge
{
    public class LobbyEnteringPortImp : NCPresentationPort, NCLobbyEnteringPort
    {
        public LobbyEnteringPortImp(NeighborhoodChallengeManager challengeManager) : base(challengeManager)
        {
        }

        public void Wait()
        {
            waitingScreenController.BegingWaiting();
        }

        public void HandleError(NCFailureResult failureResult)
        {
            HandleGeneralError(failureResult);
        }

        public void OpenLobby()
        {
            waitingScreenController.CloseWaiting();

            ServiceLocator.Find<GameTransitionManager>().GoToNCLobby(challengeManager.GetController<NCInsideLobbyController>());
        }
    }
}