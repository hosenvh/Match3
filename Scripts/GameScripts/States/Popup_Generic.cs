using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeganX;

namespace Match3
{
    public class Popup_Generic : GameState
    {

        #region fields
        [SerializeField]
        private LocalText messageText = null, buttonText = null;

        private Action buttonCallback;
        private bool isBackAvailable;
        #endregion

        #region methods
        public Popup_Generic Setup(string messageString, string buttonString, System.Action buttonCallback, bool isBackAvailable = false)
        {
            messageText.SetText(messageString);
            buttonText.SetText(buttonString);

            this.buttonCallback = buttonCallback;
            this.isBackAvailable = isBackAvailable;
            return this;
        }

        public void OnButtonClick()
        {
            gameManager.fxPresenter.PlayClickAudio();
            buttonCallback();
            if (isBackAvailable)
                base.Back();
        }

        public override void Back()
        {
            if (isBackAvailable)
                base.Back();
        }
        #endregion
    }
}