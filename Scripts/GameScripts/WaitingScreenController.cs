using I2.Loc;
using UnityEngine;

public class WaitingScreenController
{

    Popup_WaitBox popup_WaitBox;

    public void BegingWaiting()
    {
        popup_WaitBox = Base.gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);
    }

    public void CloseWaiting()
    {
        if(popup_WaitBox != null)
            popup_WaitBox.Close();
        popup_WaitBox = null;
    }
}
