using System;
using I2.Loc;
using Match3.Overlay.Advertisement.Placements;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3;
using Match3.Game;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelContinuing;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Overlay.Advertisement.Placements.Implementations;
using Match3.Presentation.Advertisement;
using Match3.Presentation.Gameplay;
using Match3.Presentation.TextAdapting;
using SeganX;
using UnityEngine;


public enum GameOverPopupResult
{
    VideoResume,
    CoinResume,
    Replay,
    Exit
}

public class GameOverBoardPreviewClickedEvent : GameEvent
{
    public GameOverBoardPreviewClickedEvent()
    {
    }
}

public class Popup_GameOver : GameState
{
    public struct GameOverPopupGeneralDefinitions
    {
        public GameplayController GameplayController { get; }
        public LevelContinuingStage ContinuingData{ get; }
        public StopConditinon StopCondition{ get; }
        public int CurrentLife{ get; }

        public GameOverPopupGeneralDefinitions(GameplayController gameplayController, LevelContinuingStage continuingData, StopConditinon stopCondition, int currentLife)
        {
            this.GameplayController = gameplayController;
            this.ContinuingData = continuingData;
            this.StopCondition = stopCondition;
            this.CurrentLife = currentLife;
        }
    }

    public struct GameOverPopupResumeCallbackDefinitions
    {
        public Action<Popup_GameOver> OnResume{ get; }
        public Action<Popup_GameOver> OnVideoResume{ get; }

        public GameOverPopupResumeCallbackDefinitions(Action<Popup_GameOver> onResume, Action<Popup_GameOver> onVideoResume)
        {
            OnResume = onResume;
            OnVideoResume = onVideoResume;
        }
    }

    public struct GameOverPopupGaveUpCallbackDefinitions
    {
        public Action OnExit{ get; }
        public Action OnReplay{ get; }

        public GameOverPopupGaveUpCallbackDefinitions(Action onExit, Action onReplay)
        {
            OnExit = onExit;
            OnReplay = onReplay;
        }
    }

    public struct GameOverPopupBoardPreviewCallbackDefinitions
    {
        public Action OnPreview { get; }

        public GameOverPopupBoardPreviewCallbackDefinitions(Action onPreview)
        {
            OnPreview = onPreview;
        }
    }

    public LocalText loseMessageText;
    [SerializeField] GameObject previewButton;
    [SerializeField] GameObject returnButton;
    [SerializeField] GameObject retrySectionGameObject = null;
    [SerializeField] GameObject popupContainer;
    [SerializeField] GameObject spaceBar;
    [SerializeField] LevelContinuingDataPreseneter levelContinuingDataPreseneter = default;
    [SerializeField] ParrotAdvertisementHolder parrotAdvertisementHolder;

    GameOverPopupGeneralDefinitions generalDefinitions;
    GameOverPopupResumeCallbackDefinitions resumeCallbackDefinitions;
    GameOverPopupGaveUpCallbackDefinitions gaveUpCallbackDefinitions;
    
    public Popup_GameOver Setup(GameOverPopupGeneralDefinitions generalDefinitions, GameOverPopupResumeCallbackDefinitions resumeCallbackDefinitions, GameOverPopupGaveUpCallbackDefinitions gaveUpCallbackDefinitions)
    {
        this.generalDefinitions = generalDefinitions;
        this.resumeCallbackDefinitions = resumeCallbackDefinitions;
        this.gaveUpCallbackDefinitions = gaveUpCallbackDefinitions;

        retrySectionGameObject.SetActive(generalDefinitions.CurrentLife > 0);

        levelContinuingDataPreseneter.Setup(generalDefinitions.ContinuingData);

        var adPlacementManager = ServiceLocator.Find<AdvertisementPlacementsManager>();
        var shouldShowAdvertisementHolder = adPlacementManager.IsAvailable<ContinuingWithExtraMovesAdPlacement>();

        parrotAdvertisementHolder.gameObject.SetActive(shouldShowAdvertisementHolder);

        if (shouldShowAdvertisementHolder)
        {
            var extraMoves =  adPlacementManager.Find<ContinuingWithExtraMovesAdPlacement>().ExtraMovesForContinuing();
            parrotAdvertisementHolder.SetupForLose(string.Format(ScriptLocalization.Message_Advertisement.WatchForExtraMoves, extraMoves), extraMoves, OnVideoResumeClick);
        }

        if (generalDefinitions.StopCondition is MovementStopCondition)
            loseMessageText.SetText(ScriptLocalization.Message_GameOver.OutOfMove);
        else if (generalDefinitions.StopCondition is GasCylinderStopCondition)
            loseMessageText.SetText(ScriptLocalization.Message_GameOver.OutOfGas);
        //graphic.AnimationState.SetAnimation(0, "exp", false);

        SetActiveBoardPreviewButton(IsBoardPreviewActivate());

        return this;
    }

    private bool IsBoardPreviewActivate()
    {
        return GetGameOverServerConfigData().IsBoardPreviewActivate;

        GameOverServerConfigData GetGameOverServerConfigData()
        {
            var gameOverServerConfigData = ServiceLocator.Find<ServerConfigManager>().data.config.gameOverServerConfigData;
            if (gameOverServerConfigData == null)
                gameOverServerConfigData = new GameOverServerConfigData();

            return gameOverServerConfigData;
        }
    }

    public void SetActiveBoardPreviewButton(bool isActive)
    {
        previewButton.SetActive(isActive);
        spaceBar.SetActive(isActive);
    }

    public void OnVideoResumeClick()
    {
        // TODO: Maybe handing this in LevelContinuingSystem class like Resume with coin, only calling a callback plus playing click audio here
        ServiceLocator.Find<AdvertisementPlacementsManager>().Play<ContinuingWithExtraMovesAdPlacement>(
            argument: new GameplayControllerArgument(generalDefinitions.GameplayController),
            onSuccess: () => resumeCallbackDefinitions.OnVideoResume.Invoke(this),
            onFailure: delegate{ });
    }

    public void OnResumeClick()
    {
        gameManager.fxPresenter.PlayClickAudio();
        resumeCallbackDefinitions.OnResume(this);
    }

    public void OnExitClick()
    {
        gameManager.fxPresenter.PlayClickAudio();
        gaveUpCallbackDefinitions.OnExit();
    }

    public void OnRetryButtonClick()
    {
        gameManager.fxPresenter.PlayClickAudio();
        gaveUpCallbackDefinitions.OnReplay();
    }

    public void OnPreviewButtonClick()
    {
        gameManager.fxPresenter.PlayClickAudio();
        SetActiveLosePopup(false);
        ServiceLocator.Find<EventManager>().Propagate(new GameOverBoardPreviewClickedEvent(), this);
    }

    public void OnReturnButtonClick()
    {
        gameManager.fxPresenter.PlayClickAudio();
        SetActiveLosePopup(true);
    }

    private void SetActiveLosePopup(bool isActive)
    {
        gameManager.popupBackgroundPanel.gameObject.SetActive(isActive);
        previewButton.SetActive(isActive);
        popupContainer.SetActive(isActive);
        returnButton.SetActive(!isActive);
    }

    public override void Back()
    {
    }
}