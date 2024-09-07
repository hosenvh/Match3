using System;
using I2.Loc;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.NeighborhoodChallenge;
using Match3.Presentation.Gameplay;
using Match3.Game.Gameplay.SubSystems.General;

namespace Match3.Presentation.NeighborhoodChallenge
{
    public class NeighborhoodChallengeGameplayState : GameplayState
    {


        protected override LifeConsumer CreateLifeConsumer()
        {
            return new NeighborhoodChallengeLifeConsumer();
        }

        protected override void PreSetup(BoardConfig boardConfig, int levelIndex)
        {
            // Nothing yet
        }

        protected override void PostSetup(GameplayController gameplayController, BoardConfig boardConfig, int levelIndex)
        {
            // Nothing yet
        }

        public override void HandleInternalPreGameEnding(LevelResult result, StopConditinon losingCause, int score)
        {
            var ncManager = ServiceLocator.Find<NeighborhoodChallengeManager>();

            if (result == LevelResult.Win)
            {
                RegisterWinLevelEndingTask(
                    new GenericLevelEndingTask((Action onComplete) => HandleLevelWining(score, ncManager, onComplete)),
                    priority: 100,
                    id: "NCOpenWinPopup");
            }
            else
            {
                RegisterLoseLevelEndingTask(
                    new GenericLevelEndingTask(
                        action: (Action onComplete) =>
                        {
                            SetupGameOverPopup(
                                losingCause,
                                gameManager.OpenPopupByPath<Popup_GameOver>("Popup_NeighborHoodChallengeGameOver"),
                                ncManager.Ticket().CurrentValue(),
                                exitAction: ncManager.GetController<NCLobbyEnteringController>().EnterLobby);
                            onComplete();
                        }), 
                    priority: 100, 
                    id: "NCOpenGameOverPopup");
            }
        }

        private void HandleLevelWining(int score, NeighborhoodChallengeManager ncManager, Action onComplete)
        {
            var popup = gameManager.OpenPopup<Popup_NeighborhoodChallengeWin>();

            ncManager.HandleLevelWinning();

            popup.Setup(
                gameplayController,
                score,
                onExit: () =>
                {
                    ncManager.GetController<NCLobbyEnteringController>().EnterLobby();
                });

            ncManager.Replace<LevelScoringPortImp>(new LevelScoringPortImp(ncManager, popup));
            ncManager.GetController<NCLevelScoringController>().SubmitLevelScore(score);

            onComplete();
        }

        public override void OnFinishGameClick()
        {
            gameManager.fxPresenter.PlayClickAudio();
            AnalyticsManager.SendEvent(new AnalyticsData_Ingame_In_Level_Back("back"));

            var texts = new Popup_ConfirmBox.ConfirmPopupTexts(ScriptLocalization.Message_NeighborhoodChallenge.ExitGamePlay, ScriptLocalization.Message_GamePlay.Resume, ScriptLocalization.UI_General.Exit);
            if(IsHeartDecremented())
            {
                OpenLevelAbortConfirmPopup(
                    texts,
                    onResult: shouldContinueLevel =>
                    {
                        if (!shouldContinueLevel)
                            OpenLevelAbortSecondaryConfirmPopup(
                                texts,
                                onResult: shouldContinue =>
                                {
                                    if (!shouldContinue)
                                        AbortLevel();
                                });
                    });
            }
            else
                AbortLevel();
        }

        public override void ReloadLevel()
        {
            ServiceLocator.Find<NeighborhoodChallengeManager>().ReloadCurrentLevel();
        }

        public override void HandleRewardDoubling(int addedScore)
        {
        }

        public void AbortLevel()
        {
            ServiceLocator.Find<EventManager>().Propagate(new LevelAbortedEvent(), this);
            //AnalyticsManager.SendEvent(new AnalyticsData_LevelResult_Back(boardPresenter.UsedExplosiveCount));
            gameManager.joySystem.SetPlayerJoy(false);
            ServiceLocator.Find<NeighborhoodChallengeManager>().GetController<NCLobbyEnteringController>().EnterLobby();
        }

        private bool IsHeartDecremented()
        {
            return gameplayController.GetSystem<LevelStartResourceConsumingSystem>().IsLevelStartResourcesConsumed;
        }
    }
}