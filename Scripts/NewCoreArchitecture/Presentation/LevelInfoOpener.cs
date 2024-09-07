using System;
using I2.Loc;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.NeighborhoodChallenge;


namespace Match3.Presentation
{
    public class LevelInfoOpener
    {
        public void TryToOpenLevelInfo(Action onPurchase)
        {
            var gameManager = Base.gameManager;
            
            if (gameManager.profiler.LifeCount > 0)
            {
                if (!gameManager.IsPlayerFinishedMainCampaign)
                {
                    var levelConfig = gameManager.levelManager.GetLevelConfig(gameManager.profiler.LastUnlockedLevel);
                    var lastUnlockedLevel = gameManager.profiler.LastUnlockedLevel;

                    if (levelConfig.HasChallengeLevel())
                        gameManager.OpenPopup<Popup_LevelInfo_ChallengeLevel>()
                            .Setup(levelConfig, lastUnlockedLevel, onPurchase, OnPlay);
                    else
                        gameManager.OpenPopup<Popup_LevelInfo>()
                            .Setup(levelConfig, lastUnlockedLevel, onPurchase, OnPlay);

                    void OnPlay()
                    {
                        gameManager.fxPresenter.PlayClickAudio();
                        AnalyticsManager.SendEvent(new AnalyticsData_LevelInfo_Start());
                        ServiceLocator.Find<GameTransitionManager>().GoToLevel(
                            gameManager.levelManager.GetLevelConfig(gameManager.profiler.LastUnlockedLevel),
                            gameManager.profiler.LastUnlockedLevel);
                    }
                }
                else
                {
                    var isNcEnabled = ServiceLocator.Find<NeighborhoodChallengeManager>().IsEnabled();
                    var finishedGameDescription = isNcEnabled
                        ? ScriptLocalization.Message_Campaign.YouFinishedAllLevelsWithNcOffer
                        : ScriptLocalization.Message_Campaign.YouFinishedAllLevels;

                    var confirmButtonText = isNcEnabled
                        ? ScriptLocalization.UI_NeighborhoodChallenge.NeighborhoodChallengeName
                        : ScriptLocalization.UI_General.Ok;

                    var cancelButtonText = isNcEnabled ? ScriptLocalization.UI_General.LeaveIt.ToString() : "";

                    gameManager.OpenPopup<Popup_ConfirmBox>()
                        .Setup(finishedGameDescription,
                            confirmButtonText, cancelButtonText, true, OnResult);
                
                    void OnResult(bool result)
                    {
                        if(result && isNcEnabled)
                            OpenNeighborhoodChallenge();
                    }
                }
            }
            else
                gameManager.OpenPopup<Popup_Life>().Setup(onPurchase);
        }
        
        private void OpenNeighborhoodChallenge()
        {
            var NCManager = ServiceLocator.Find<NeighborhoodChallengeManager>();
            NCManager.GetController<NCLobbyEnteringController>().EnterLobby();
        }
    }
}