using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeganX;

public class Popup_WaitBox : GameState
{
    #region fields
    [SerializeField]
    private LocalText messageText = null;
    #endregion

    #region methods
    public Popup_WaitBox Setup(string messageString)
    {
        messageText.SetText(messageString);
        return this;
    }

    public override void Back()
    {
    }

    public new void Close()
    {
        base.Back();
    }
    #endregion
}