using System;
using I2.Loc;
using Match3.Game.ReferralMarketing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



namespace Match3.Presentation.ReferralMarketing
{

    public class EnterReferralCodePanelController : MonoBehaviour
    {

        public Button submitButton;
        public TMP_InputField inputField;

        public Action<SuccessfulReferredData> onSucceed;
        public Action<UseReferralCodeFailureReason> onFailed;

        private string segmentTag = "";
        private ReferralCenterServerController serverController;

        private Action onCloseAction;
        
        public void Setup(ReferralCenterServerController serverController, Action<SuccessfulReferredData> onSucceed, Action<UseReferralCodeFailureReason> onFailed)
        {
            this.serverController = serverController;
            this.onSucceed = onSucceed;
            this.onFailed = onFailed;
        }

        public void ChangeSubmitCodeButtonFunctionality(UnityAction onButtonPressed)
        {
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(onButtonPressed);
        }
        
        public void SetOnCloseExtraAction(Action onClose)
        {
            onCloseAction = onClose;
        }
        
        private void Update()
        {
            submitButton.interactable = !string.IsNullOrEmpty(inputField.text);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            onCloseAction?.Invoke();
        }

        public string GetEnteredCodeSegmentTag()
        {
            return segmentTag.ToUpper();
        }
        
        public void SubmitCode()
        {
            var popupWaitBox = Base.gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);
            
            string referralCode = inputField.text;
            
            if (inputField.text.Length > 5)
            {
                referralCode = inputField.text.Substring(0, 5);
                segmentTag = inputField.text.Substring(5);
            }
            
            serverController.UseReferralCode(referralCode, data =>
            {
                Base.gameManager.ClosePopup(popupWaitBox);
                onSucceed(data);
            }, reason =>
            {
                Base.gameManager.ClosePopup(popupWaitBox);
                onFailed(reason);
            });
        }
        
        
        
    }

}


