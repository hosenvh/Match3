using I2.Loc;
using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Game.Gameplay.RainbowMechanic;
using Match3.Presentation.TransitionEffects;

namespace Match3.Presentation.Gameplay
{
    public class CampaignGameplayState : GameplayState
    {
        public bool IsChallengeLevel { get; private set; }

        private ChallengeLevelEventListener challengeLevelEventListener;

        protected override LifeConsumer CreateLifeConsumer()
        {
            return new CampaignLifeConsumer();
        }

        protected override void PreSetup(BoardConfig boardConfig, int levelIndex)
        {
            var serverLevelsConfig = ServiceLocator.Find<ServerConfigManager>().data.config.serverLevelsConfig;
            if (serverLevelsConfig != null)
                for (var i = serverLevelsConfig.Length - 1; i >= 0; i--)
                {
                    if (serverLevelsConfig[i].levelIndex != levelIndex) continue;
                    this.boardConfig.levelConfig.maxMove = serverLevelsConfig[i].maxMove;
                    break;
                }

            boardConfig.TrySetChallengeLevelActive();
            if(boardConfig.IsChallengeLevelActive())
            {
                challengeLevelEventListener = new ChallengeLevelEventListener(boardConfig, this);
                RegisterWinLevelEndingTask(new ChallengeLevelRewardingEndingTask(boardConfig), 9, "ChallengeLevelRewarding");
                IsChallengeLevel = true;
            }
        }

        protected override void PostSetup(GameplayController gameplayController, BoardConfig boardConfig, int levelIndex)
        {
            // TODO: Move this to config
            if (levelIndex <= 2)
                gameplayController.RemoveSystem<RainbowGenerationSystem>();
        }


        public override void HandleInternalPreGameEnding(LevelResult result, StopConditinon losingCause, int score)
        {
            if (result == LevelResult.Win)
            {
                Base.gameManager.joySystem.SetPlayerJoy(true);
                gameManager.profiler.OnWin(score);

                //TODO: Check to find out it's better to register piggy bank ending task from somewhere else or not!
                RegisterWinLevelEndingTask(new PiggyBankRewardCollectingTask(score), 5, "PiggyBankCollectingCoin");
                RegisterWinLevelEndingTask(new CampaignOpenWinPopupTask(gameplayController, score), 10, "OpenWinPopup");
                RegisterWinLevelEndingTask(new GenericLevelEndingTask(delegate(Action onComplete)
                {
                    OpenState_Map();
                    onComplete();
                }), 100, "OpenStateMapEndingTask");
            }
            else
            {
                gameManager.joySystem.SetPlayerJoy(false);

                RegisterLoseLevelEndingTask(new GenericLevelEndingTask(delegate(Action onComplete)
                {
                    SetupGameOverPopup(
                        losingCause,
                        gameManager.OpenPopup<Popup_GameOver>(),
                        gameManager.profiler.LifeCount,
                        exitAction: OpenState_Map
                    );
                    onComplete();
                }), 100, "OpenGameOverLevelEndingTask");
            }
        }

        
        public override void HandleRewardDoubling(int addedScore)
        {
            gameManager.profiler.ChangeCoin(addedScore, "win");
        }

        public override void OnFinishGameClick()
        {
            gameManager.fxPresenter.PlayClickAudio();
            AnalyticsManager.SendEvent(new AnalyticsData_Ingame_In_Level_Back("back"));

            string message = HasInfiniteLife() ? ScriptLocalization.Message_Campaign.ExitGameplayWhileHavingInfiniteLife : ScriptLocalization.Message_Campaign.ExitGameplay;
            var texts = new Popup_ConfirmBox.ConfirmPopupTexts(message, ScriptLocalization.Message_GamePlay.Resume, ScriptLocalization.UI_General.Exit);
            if (IsLevelStartResourcesConsumed())
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
            else
                AbortLevel();
        }

        private void AbortLevel()
        {
            ServiceLocator.Find<EventManager>().Propagate(new LevelAbortedEvent(), this);
            //AnalyticsManager.SendEvent(new AnalyticsData_LevelResult_Back(boardPresenter.UsedExplosiveCount));
            gameManager.joySystem.SetPlayerJoy(false);
            OpenState_Map();
        }

        private void OpenState_Map()
        {
            ServiceLocator.Find<GameTransitionManager>().GoToLastMap<DarkInTransitionEffect>();
        }

        public override void ReloadLevel()
        {
            ServiceLocator.Find<GameTransitionManager>().GoToLevel(boardConfig, selectedLevelIndex);
        }

        private bool HasInfiniteLife()
        {
            return ServiceLocator.Find<ILife>().IsInInfiniteLife();
        }

        private void OnDestroy()
        {
            challengeLevelEventListener?.Dispose();
        }
    }
}