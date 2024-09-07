using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game.TaskManagement;
using Match3.Presentation.Gameplay;
using Match3.Presentation.HUD;
using Match3.Presentation.Rewards;
using static Match3.Presentation.Gameplay.GameplayState.RewardLossWarning;
using static Match3.Presentation.Gameplay.GameplayState;

namespace Match3.Game.Gameplay
{

    public class ChallengeLevelEventListener : EventListener
    {

        private BoardConfig boardConfig;
        private GameplayState gameplayState;

        private bool challengeStarted = false;
        
        public ChallengeLevelEventListener(BoardConfig boardConfig, GameplayState gameplayState)
        {
            this.boardConfig = boardConfig;
            this.gameplayState = gameplayState;

            ServiceLocator.Find<EventManager>().Register(this);
        }

        public void Dispose()
        {
            ServiceLocator.Find<EventManager>().UnRegister(this);
        }
        
        public void OnEvent(GameEvent evt, object sender)
        {
            switch (evt)
            {
                case LevelStartResourceConsumingEvent _:
                    if (!challengeStarted)
                    {
                        boardConfig.SetChallengeLevelPlayed();
                        gameplayState.RegisterRewardLossWarning(CreateChallengeLevelRewardLossWarning());
                        challengeStarted = true;   
                    }
                    break;
                case PopupWinSetupEvent popupWinOpenedEvent:
                    var rewards = boardConfig.challengeLevelConfig.GetRewards();
                    foreach (var reward in rewards)
                    {
                        var rewardPresentation =
                            new RewardPresentationFactory().GenerateRewardPresentation(RewardPresentationType.BottomValue, reward, popupWinOpenedEvent.winPopup.transform);
                        popupWinOpenedEvent.winPopup.AddRewardPresentationAtBottom(rewardPresentation, 0);
                    }
                    break;
            }
        }

        private RewardLossWarning CreateChallengeLevelRewardLossWarning()
        {
            var confirmPopupOpeningHandler = new SecondaryConfirmPopupOpener((texts, onResult) =>
                Base.gameManager.OpenPopup<Popup_ChallengeLevelRewardLosing_LevelExitConfirm>().Setup(texts, onResult));
            
            var gameOverPopupOpeningHandler = new SecondaryGameOverPopupOpener(
                (generalDefinitions, resumeCallbackDefinitions, gaveUpCallbackDefinitions) =>
                    Base.gameManager.OpenPopup<Popup_ChallengeLevelRewardLosing_GameOver>().Setup(generalDefinitions,
                        resumeCallbackDefinitions, gaveUpCallbackDefinitions));
            
            return new RewardLossWarning(priority: RewardLossWarningsPriorities.ChallengeLevel, confirmPopupOpeningHandler, gameOverPopupOpeningHandler);
        }
        
    }
    
}


