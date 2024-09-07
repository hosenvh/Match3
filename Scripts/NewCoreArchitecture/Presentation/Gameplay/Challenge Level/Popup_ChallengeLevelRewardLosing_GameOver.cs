using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;


public class ChallengeLevelGameOverPopupResultEvent : GameEvent
{
    public GameOverPopupResult Result;

    public ChallengeLevelGameOverPopupResultEvent(GameOverPopupResult result)
    {
        Result = result;
    }
}

public class Popup_ChallengeLevelRewardLosing_GameOver : Popup_GameOver
{
    public new Popup_GameOver Setup(GameOverPopupGeneralDefinitions generalDefinitions, GameOverPopupResumeCallbackDefinitions resumeCallbackDefinitions, GameOverPopupGaveUpCallbackDefinitions gaveUpCallbackDefinitions)
    {
        GameOverPopupResumeCallbackDefinitions resumeCallbackDef = new GameOverPopupResumeCallbackDefinitions(
            popupGameOver =>
            {
                resumeCallbackDefinitions.OnResume(popupGameOver);
                SendEvent(GameOverPopupResult.CoinResume);
            }, popupGameOver =>
            {
                resumeCallbackDefinitions.OnVideoResume(popupGameOver);
                SendEvent(GameOverPopupResult.VideoResume);
            });

        GameOverPopupGaveUpCallbackDefinitions gaveUpCallbackDef = new GameOverPopupGaveUpCallbackDefinitions(() =>
        {
            gaveUpCallbackDefinitions.OnExit();
            SendEvent(GameOverPopupResult.Exit);
        }, () =>
        {
            gaveUpCallbackDefinitions.OnReplay();
            SendEvent(GameOverPopupResult.Replay);
        });
        
        return base.Setup(generalDefinitions, resumeCallbackDef, gaveUpCallbackDef);
    }

    public void SendEvent(GameOverPopupResult result)
    {
        ServiceLocator.Find<EventManager>().Propagate(new ChallengeLevelGameOverPopupResultEvent(result), this);
    }
}
