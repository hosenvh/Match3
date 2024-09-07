using I2.Loc;
using Match3.Overlay.Advertisement.Base;
using static Base;


namespace Match3.Overlay.Advertisement.Service.Session
{
    public class AdvertisementSessionPresentation
    {
        private Popup_WaitBox currentOpenWaitingPopup;
        private bool shouldShowSessionPresentation;

        public void Setup(bool shouldShowSessionPresentation)
        {
            this.shouldShowSessionPresentation = shouldShowSessionPresentation;
        }

        public void OpenWaitingPopup()
        {
            if (shouldShowSessionPresentation)
                currentOpenWaitingPopup = gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);
        }

        public void CloseWaitingPopup()
        {
            if (currentOpenWaitingPopup != null)
                currentOpenWaitingPopup.Close();
        }

        public void ShowPopupForRewardedSkipped()
        {
            gameManager.OpenPopup<Popup_ConfirmBox>().Setup(messageString: ScriptLocalization.Message_Advertisement.WatchVideoCompletely, confirmString: ScriptLocalization.UI_General.Ok, cancelString: null, closeOnConfirm: true);
        }

        public void ShowFailurePopup(AdFailReason reason)
        {
            if (shouldShowSessionPresentation == false)
                return;
            if (reason == AdFailReason.TimeOut)
                ShowSingleButtonConfirmPopup(message: ScriptLocalization.Message_Advertisement.Timeout);
            else if (reason == AdFailReason.NoConnection)
                ShowSingleButtonConfirmPopup(message: ScriptLocalization.Message_Network.AskForInternet);
            else
                ShowSingleButtonConfirmPopup(message: ScriptLocalization.Message_Advertisement.NoAvailableVideo);

            void ShowSingleButtonConfirmPopup(string message)
            {
                gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                    message,
                    confirmString: ScriptLocalization.UI_General.Ok,
                    cancelString: null,
                    closeOnConfirm: true);
            }
        }
    }
}