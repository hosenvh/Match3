using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;


public class ChallengeLevelExitConfirmEvent : GameEvent
{
    public bool Result;
    public ChallengeLevelExitConfirmEvent(bool result)
    {
        Result = result;
    }
}

public class Popup_ChallengeLevelRewardLosing_LevelExitConfirm : Popup_ConfirmBox
{
    public Popup_ConfirmBox Setup(ConfirmPopupTexts texts, Action<bool> onResult)
    {
        return Setup(texts, closeOnConfirm: true, result =>
        {
            onResult(result);
            ServiceLocator.Find<EventManager>().Propagate(new ChallengeLevelExitConfirmEvent(result), this);
        });
    }
}
