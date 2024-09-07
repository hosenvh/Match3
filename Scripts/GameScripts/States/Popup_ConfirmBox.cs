using System;
using UnityEngine;
using SeganX;


public class Popup_ConfirmBox : GameState
{
    public class ConfirmPopupTexts
    {
        public string Message { get; }
        public string ConfirmText { get; }
        public string CancelText { get; }

        public ConfirmPopupTexts(string message, string confirmText, string cancelText)
        {
            Message = message;
            ConfirmText = confirmText;
            CancelText = cancelText;
        }
    }

    #region fields
    [SerializeField]
    private LocalText messageText = null, confirmText = null, cancelText = null;
    [SerializeField]
    private GameObject cancelButtonGameObject = null;

    private bool closeOnConfirm;
    System.Action<bool> OnResult;
    #endregion

    #region methods

    public Popup_ConfirmBox Setup(ConfirmPopupTexts texts, bool closeOnConfirm)
    {
        return Setup(texts, closeOnConfirm, delegate { });
    }

    public Popup_ConfirmBox Setup(ConfirmPopupTexts texts, bool closeOnConfirm, Action<bool> onResult)
    {
        return Setup(texts.Message, texts.ConfirmText, texts.CancelText, closeOnConfirm, onResult);
    }

    public Popup_ConfirmBox Setup(string messageString, string confirmString, string cancelString, bool closeOnConfirm)
    {
        return Setup(messageString, confirmString, cancelString, closeOnConfirm, onResult: delegate { });
    }

    public Popup_ConfirmBox Setup(string messageString, string confirmString, string cancelString, bool closeOnConfirm, Action<bool> onResult)
    {
        messageText.SetText(messageString);
        confirmText.SetText(confirmString);
        if (string.IsNullOrEmpty(cancelString))
            cancelButtonGameObject.SetActive(false);
        else
            cancelText.SetText(cancelString);
        this.closeOnConfirm = closeOnConfirm;
        this.OnResult = onResult;
        return this;
    }

    public void OnButtonClick(bool confirm)
    {
        gameManager.fxPresenter.PlayClickAudio();
        if (!confirm || closeOnConfirm)
            base.Back();
        if (OnResult != null)
            OnResult(confirm);
    }

    public override void Back()
    {
    }
    #endregion
}