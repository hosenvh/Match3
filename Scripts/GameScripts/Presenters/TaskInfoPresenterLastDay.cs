using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;


namespace Match3
{
    public class TaskInfoPresenterLastDay : MonoBehaviour
    {

        Action callback = null;
        #region public methods
        public TaskInfoPresenterLastDay Setup(Action callback)
        {
            this.callback = callback;
            return this;
        }

        public void ButtonCallback()
        {
            Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Tasks.NoMoreTask, ScriptLocalization.UI_General.Ok, null, true, (confirm) =>
            {
                callback?.Invoke();
            });
        }
        #endregion
    }
}