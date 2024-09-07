using System;
using I2.Loc;
using Match3.Game.NeighborhoodChallenge;
using Match3.Game.NeighborhoodChallenge.RequestHandling;

namespace Match3.Presentation.NeighborhoodChallenge
{
    public class NCPresentationPort
    {
        protected NeighborhoodChallengeManager challengeManager;

        protected WaitingScreenController waitingScreenController;

        public NCPresentationPort(NeighborhoodChallengeManager challengeManager)
        {
            this.challengeManager = challengeManager;
            waitingScreenController = new WaitingScreenController();
        }


        protected void HandleGeneralError(NCFailureResult failureResult)
        {
            waitingScreenController.CloseWaiting();
            switch (failureResult)
            {
                case InternalServerErrorFailureResult result:
                    Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Network.ServerError, ScriptLocalization.UI_General.Ok, "", true, (r) => Confirm(result));
                    break;

                case TimeOutFailureResult result:
                    Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Network.ServerNotResponse, ScriptLocalization.UI_General.Again, ScriptLocalization.UI_General.Cancel, true, (r) => HandleRetry(r, result));
                    break;

                case NoInternetConnectionFailureResult result:
                    Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Network.InternetIsNotConnect, ScriptLocalization.UI_General.Again, ScriptLocalization.UI_General.Cancel, true, (r) => HandleRetry(r, result));
                    break;
                case NoChallengeIsActiveForGame result:
                    Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_NeighborhoodChallenge.NcIsClose, ScriptLocalization.UI_General.Ok, "", true, (r) => Confirm(result));
                    break;

                case PlayerBannedFailureResult result:
                    Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_NeighborhoodChallenge.YouAreBan, ScriptLocalization.UI_General.Oh, "", true, (r) => Confirm(result));
                    break;
            }
        }

        private void Confirm(BaseFailureResult result)
        {
            result.confirmAction.Invoke();
        }

        private void HandleRetry(bool userConfirm, RetriableFailureResult result)
        {
            if (userConfirm)
            {
                waitingScreenController.BegingWaiting();
                result.retryAction.Invoke();
            }
            else
            {
                result.confirmAction.Invoke();
            }
        }

        protected void BeginWaiting()
        {
            waitingScreenController.BegingWaiting();
        }

        protected void EndWaiting()
        {
            waitingScreenController.CloseWaiting();
        }
    }
}