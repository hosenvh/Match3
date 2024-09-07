using UnityEngine;
using SeganX;
using Match3.Game.Gameplay;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Initialization;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Presentation.Gameplay.LogicPresentationHandlers;
using Match3.Game.Gameplay.SubSystems.LevelContinuing;
using System;
using System.Collections.Generic;
using I2.Loc;
using Match3.Foundation.Base.TaskManagement;
using Match3.Game.Effects.GamePlayEffects;
using Match3.Game.MainShop;
using static Popup_GameOver;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Presentation.Effects.PresentationEffects;


namespace Match3.Presentation.Gameplay
{
    public class LevelGaveUpEvent : GameEvent
    {
    }

    public class LevelAbortedEvent : GameEvent
    {
    }

    public class LevelPresentationInitializedEvent : GameEvent
    {
        public readonly GameplayController gpc;
        public readonly int levelIndex;

        public LevelPresentationInitializedEvent(GameplayController gpc, int levelIndex)
        {
            this.gpc = gpc;
            this.levelIndex = levelIndex;
        }
    }

    public abstract class GameplayState : GameState
    {
        public class RewardLossWarning
        {
            public delegate Popup_ConfirmBox SecondaryConfirmPopupOpener(Popup_ConfirmBox.ConfirmPopupTexts texts, Action<bool> onResult);
            public delegate Popup_GameOver SecondaryGameOverPopupOpener(
                GameOverPopupGeneralDefinitions generalDefinitions,
                GameOverPopupResumeCallbackDefinitions resumeCallbackDefinitions,
                GameOverPopupGaveUpCallbackDefinitions gaveUpCallbackDefinitions);

            public int Priority { get; }
            public SecondaryConfirmPopupOpener SecondaryConfirmPopupOpeningHandler { get; }
            public SecondaryGameOverPopupOpener SecondaryGameOverPopupOpeningHandler { get; }

            public RewardLossWarning(int priority, SecondaryConfirmPopupOpener secondaryConfirmPopupOpeningHandler, SecondaryGameOverPopupOpener secondaryGameOverPopupOpeningHandler)
            {
                Priority = priority;
                SecondaryConfirmPopupOpeningHandler = secondaryConfirmPopupOpeningHandler;
                SecondaryGameOverPopupOpeningHandler = secondaryGameOverPopupOpeningHandler;
            }
        }

        public GameplayStateRoot root;
        public GameplayController gameplayController;

        protected int retryCount = 0, selectedLevelIndex;
        protected BoardConfig boardConfig = null;

        private readonly SequentialSortedTaskChain winLevelEndingTaskChain = new SequentialSortedTaskChain();
        private readonly SequentialSortedTaskChain loseLevelEndingTaskChain = new SequentialSortedTaskChain();

        private readonly List<RewardLossWarning> rewardLossWarnings = new List<RewardLossWarning>();
        private RewardLossWarning CurrentRewardLossWarning => rewardLossWarnings.Count > 0 ?  rewardLossWarnings[0] : null;

        private GameplayCameraShakeController cameraShakeController;
        private float gameStartTime;
        
        public float BoardScale { get; private set; }

        public GameplayState Setup(BoardConfig boardConfig, int levelIndex)
        {
            ServiceLocator.Find<GameplaySoundManager>().StopAll();

            SetupBoardScale(boardConfig);

            this.boardConfig = boardConfig;
            selectedLevelIndex = levelIndex;
            //gameManager.state_Game = this;
            gameStartTime = Time.time;
            AnalyticsDataMaker.Setup(GetInGameTime, () =>{ return -1; });
            gameManager.musicManager.StartSceneMusic(1, 2f);

            AnalyticsManager.SendEvent(new AnalyticsData_Snapshot_Open());
            
            PreSetup(boardConfig, levelIndex);

            root.levelDifficultyUIController.Setup(boardConfig);
            
            gameplayController = new GameplayController();

            foreach(var handler in root.generalPresentationHandlersContainer.GetComponentsInChildren<PresentationHandler>())
                gameplayController.AddPresentationHandler(handler);

            new GameplayInitializer().Initialize(gameplayController, boardConfig, CreateLifeConsumer());

            root.boardPresenter.Setup(gameplayController);

            root.levelStatePresentationController.Setup(gameplayController, levelIndex);

            root.powerUpContainer.Setup(this);

            gameManager.profiler.PlayCount++;

            foreach (var controller in root.generalPresentationHandlersContainer.GetComponentsInChildren<PresentationController>())
                controller.Setup(this);

            root.hedgesBounceController.Setup(gameplayController);

            cameraShakeController = new GameplayCameraShakeController(root.generalPresentationHandlersContainer.GetComponentInChildren<GameplayCameraShakePresentation>(),gameplayController);
            
            PostSetup(gameplayController, boardConfig, levelIndex);

            ServiceLocator.Find<EventManager>().Propagate(new LevelPresentationInitializedEvent(gameplayController, levelIndex), this);
            return this;
        }

        private void SetupBoardScale(BoardConfig boardConfig)
        {
            float screenRatio = Screen.width / (float)Screen.height;

            var originalYResolution = root.gameCanvasScaler.referenceResolution.y;

            if (boardConfig.boardWidth == 12 && screenRatio < 1.6f)
            {
                root.gameCanvasScaler.referenceResolution = new Vector2(0, 1350 - 450 * screenRatio);
                root.otherCanvasScaler.referenceResolution = new Vector2(0, 1350 - 450 * screenRatio);
                root.backCanvasScaler.referenceResolution = new Vector2(0, 1350 - 450 * screenRatio);
            }
            else if (boardConfig.boardWidth == 11 && screenRatio < 1.5f)
            {
                root.gameCanvasScaler.referenceResolution = new Vector2(0, 1350 - 480 * screenRatio);
                root.otherCanvasScaler.referenceResolution = new Vector2(0, 1350 - 480 * screenRatio);
                root.backCanvasScaler.referenceResolution = new Vector2(0, 1350 - 480 * screenRatio);
            }
            else if (boardConfig.boardWidth == 10 && screenRatio < 1.4f)
            {
                root.gameCanvasScaler.referenceResolution = new Vector2(0, 1350 - 510 * screenRatio);
                root.otherCanvasScaler.referenceResolution = new Vector2(0, 1350 - 510 * screenRatio);
                root.backCanvasScaler.referenceResolution = new Vector2(0, 1350 - 510 * screenRatio);
            }

            BoardScale = originalYResolution / root.gameCanvasScaler.referenceResolution.y;
        }

        public void RegisterRewardLossWarning(RewardLossWarning rewardLossWarning)
        {
            rewardLossWarnings.Add(rewardLossWarning);
            rewardLossWarnings.Sort((a, b) => a.Priority < b.Priority ? -1 : 1);
        }

        public void UnRegisterRewardLossWarning(RewardLossWarning rewardLossWarning)
        {
            rewardLossWarnings.Remove(rewardLossWarning);
        }

        protected bool IsAnyRewardLossWarningRegistered()
        {
            return CurrentRewardLossWarning != null;
        }

        // TODO: This must be in logic layer not here.
        protected abstract LifeConsumer CreateLifeConsumer();

        protected abstract void PreSetup(BoardConfig boardConfig, int levelIndex);
        protected abstract void PostSetup(GameplayController gameplayController, BoardConfig boardConfig, int levelIndex);

        protected void SetupGameOverPopup(StopConditinon losingCause, Popup_GameOver popup, int currentLife, Action exitAction)
        {
            var levelContinuingSystem = gameplayController.GetSystem<LevelContinuingSystem>();
            var continuingData = levelContinuingSystem.CurrentStage();

            var generalDefinitions = new GameOverPopupGeneralDefinitions(gameplayController, continuingData, losingCause, currentLife);

            var resumeCallbackDefinitions = new GameOverPopupResumeCallbackDefinitions(
                onResume: gameoverPopup => HandleGameOverPopupResume(gameoverPopup, levelContinuingSystem),
                onVideoResume: HandleGameOverPopupVideoResume);

            var finalGaveUpCallBackDefinitions = new GameOverPopupGaveUpCallbackDefinitions(
                onExit: () => HandleGameOverPopupExit(exitAction),
                onReplay: HandleGameOverPopupReplay);

            var currentGaveUpCallBackDefinitions = new GameOverPopupGaveUpCallbackDefinitions(
                onExit: () => OpenLevelGaveUpSecondaryWarningPopup(popup, generalDefinitions, resumeCallbackDefinitions, finalGaveUpCallBackDefinitions, onNoSecondaryWarning: finalGaveUpCallBackDefinitions.OnExit),
                onReplay: () => OpenLevelGaveUpSecondaryWarningPopup(popup, generalDefinitions, resumeCallbackDefinitions, finalGaveUpCallBackDefinitions, onNoSecondaryWarning: finalGaveUpCallBackDefinitions.OnReplay));

            popup.Setup(generalDefinitions, resumeCallbackDefinitions, currentGaveUpCallBackDefinitions);
        }

        private void HandleGameOverPopupResume(Popup_GameOver popup, LevelContinuingSystem levelContinuingSystem)
        {
            levelContinuingSystem.TryContinueLevel(
                onSuccess: () => popup.Close(),
                onNotEnoughCoin: () =>
                {
                    gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Purchase.NotEnoughCoin, ScriptLocalization.UI_General.Ok, null, true, (confirm) =>
                    {
                        gameManager.shopCreator.TrySetupMainShop("game");
                    });
                }
            );
        }

        private void HandleGameOverPopupVideoResume(Popup_GameOver popup)
        {
            gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Advertisement.YouGotExtraMove, ScriptLocalization.UI_General.Ok, null, true, (confirm) =>
            {
                popup.Close();
                ServiceLocator.Find<EventManager>().Propagate(new LevelResumeWithAdsExtraMoveEvent(), this);
            });
        }

        private void HandleGameOverPopupExit(Action exitAction)
        {
            ServiceLocator.Find<EventManager>().Propagate(new LevelGaveUpEvent(), this);
            exitAction.Invoke();
        }

        private void HandleGameOverPopupReplay()
        {
            gameManager.OpenPopup<Popup_LevelInfo>().Setup(boardConfig, selectedLevelIndex, null, Replay);
        }

        private void OpenLevelGaveUpSecondaryWarningPopup(Popup_GameOver currentGameOverPopup, GameOverPopupGeneralDefinitions generalDefinitions, GameOverPopupResumeCallbackDefinitions resumeCallbackDefinitions, GameOverPopupGaveUpCallbackDefinitions gaveUpCallbackDefinitions, Action onNoSecondaryWarning)
        {
            if (IsAnyRewardLossWarningRegistered())
            {
                currentGameOverPopup.Close();
                CurrentRewardLossWarning.SecondaryGameOverPopupOpeningHandler.Invoke(generalDefinitions, resumeCallbackDefinitions, gaveUpCallbackDefinitions);
            }
            else
                onNoSecondaryWarning.Invoke();
        }

        private void Update()
        {
            gameplayController.Update(Time.deltaTime);
        }

        public abstract void OnFinishGameClick();

        public override void Back()
        {
        }

        public void Replay()
        {
            ServiceLocator.Find<EventManager>().Propagate(new LevelRetriedEvent(), this);
            ReloadLevel();
        }

        public int CurrentLevelIndex()
        {
            return selectedLevelIndex;
        }

        public int GetInGameTime() { return Mathf.FloorToInt(Time.time - gameStartTime); }

        public void RegisterWinLevelEndingTask(LevelEndingTask task, int priority, string id)
        {
            winLevelEndingTaskChain.AddTask(task, priority, id);
        }

        public void RegisterLoseLevelEndingTask(LevelEndingTask task, int priority, string id)
        {
            loseLevelEndingTaskChain.AddTask(task, priority, id);
        }
        
        public void HandleGameEnding(LevelResult result, StopConditinon losingCause, int score)
        {
            HandleInternalPreGameEnding(result, losingCause, score);
            if(result == LevelResult.Win)
                winLevelEndingTaskChain.Execute(delegate {  }, delegate {  });
            else
                loseLevelEndingTaskChain.Execute(delegate {  }, delegate {  });
        }
        
        public abstract void HandleInternalPreGameEnding(LevelResult result, StopConditinon losingCause, int score);

        
        public abstract void HandleRewardDoubling(int addedScore);

        public abstract void ReloadLevel();

        protected bool IsLevelStartResourcesConsumed()
        {
            return gameplayController.GetSystem<LevelStartResourceConsumingSystem>().IsLevelStartResourcesConsumed;
        }

        protected void OpenLevelAbortConfirmPopup(Popup_ConfirmBox.ConfirmPopupTexts texts, Action<bool> onResult)
        {
            onResult += shouldContinueLevel => AnalyticsManager.SendEvent(new AnalyticsData_Ingame_In_Level_Back(shouldContinueLevel ? "back_no" : "back_yes"));
            gameManager.OpenPopup<Popup_ConfirmBox>().Setup(texts, closeOnConfirm: true, onResult);
        }

        protected void OpenLevelAbortSecondaryConfirmPopup(Popup_ConfirmBox.ConfirmPopupTexts texts, Action<bool> onResult)
        {
            if (IsAnyRewardLossWarningRegistered())
                CurrentRewardLossWarning.SecondaryConfirmPopupOpeningHandler.Invoke(texts, onResult);
            else
                onResult.Invoke(false);
        }
    }
}