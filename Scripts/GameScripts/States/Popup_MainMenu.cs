using System;
using System.Collections.Generic;
using UnityEngine;
using SeganX;
using Match3;
using Match3.Clan.Presentation;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.EventManagement;
using Match3.CloudSave;
using Match3.Presentation.HUD;
using Match3.Foundation.Base.TaskManagement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.TaskManagement;
using Match3.Presentation;
using Match3.Presentation.MainMenu;
using Match3.Game.MainShop;
using Match3.Wardrobe.Presentation;
using UnityEngine.Serialization;
using UnityEngine.UI;


public struct MainMenuOpenEvent : GameEvent
{
    public Popup_MainMenu MainMenu { get; }

    public MainMenuOpenEvent(Popup_MainMenu mainMenu)
    {
        MainMenu = mainMenu;
    }
}

public class Popup_MainMenu : GameState, EventListener
{
    #region fields

    public HudPresentationController hudPresentationController = default;
    public MainMenuDynamicSpecialOfferController mainMenuDynamicSpecialOfferController;
    public TaskPopupMainMenuController taskPopupMainMenuController;
    public ReferralCenterMainMenuController referralCenterMainMenuController;
    public WardrobeMainMenuButton wardrobeMainMenuButton;
    public ClanMainMenuButton clanMainMenuButton;
    
    [SerializeField] private ExpandEffectPlayer expandEffectPlayer;

    [SerializeField]
    private LocalText coinCountText = null, starCountText = null, selectedLevelText = null;

    [Space(15)] 
    public Image playButtonImage;
    public List<ByDifficultyPlayButtonSprite> playButtonSprites;
    public Sprite challengePlayButtonSprite;

    [Space(10)] [SerializeField] private CloudSaveMessenger cloudSaveMessenger = default;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private List<GameObject> notifiersParents;
    #endregion
    

    #region methods
    public void Setup()
    {
        var eventManager = ServiceLocator.Find<EventManager>();
        eventManager.Register(this);
        
    }


    private void OnEnable()
    {
        ServiceLocator.Find<EventManager>().Register(this);
    }

    private void OnDisable()
    {
        ServiceLocator.Find<EventManager>().UnRegister(this);
    }


    private void Start()
    {
        EventManager eventManager = ServiceLocator.Find<EventManager>();
        eventManager.Propagate(new MainMenuOpenEvent(this), this);

        InitializePlayButton();
        UpdateGui();

        var cloudSaveService = ServiceLocator.Find<CloudSaveService>();
        if (cloudSaveService.IsServerAuthenticated && cloudSaveService.Status == AuthenticationStatus.AuthenticationFailed)
            cloudSaveService.Authenticate(delegate { });

        var mainMenuTaskChain = ServiceLocator.Find<MainMenuTaskChainService>();
        mainMenuTaskChain.AddTask(new MainMenuFirstScenarioTask(gameObject), priority: MainMenuTasksPriorities.FirstScenarioTaskPriority, "FirstScenarioTask");
        mainMenuTaskChain.AddTask(new MainMenuNotificationGiftTask(hudPresentationController), priority: MainMenuTasksPriorities.NotificationGiftTask, "NotificationGiftTask");
        mainMenuTaskChain.AddTask(new MainMenuUpdateWelcomeTask(hudPresentationController), priority: MainMenuTasksPriorities.UpdateWelcomeTask, "UpdateWelcomeTask");
        mainMenuTaskChain.AddTask(new MainMenuDifficultyRewardTask(hudPresentationController), priority: MainMenuTasksPriorities.DynamicDifficultyTask, "DynamicDifficultyTask");
        mainMenuTaskChain.AddTask(new MainMenuReferralCenterTutorialTask(), priority: MainMenuTasksPriorities.ReferralTutorialTask, "ReferralTutorialTask");
        mainMenuTaskChain.AddTask(new MainMenuOpenTaskPopupTask(taskPopupMainMenuController), priority: MainMenuTasksPriorities.OpenTaskPopupTask, "OpenTaskPopupTask");
        mainMenuTaskChain.AddTask(new MainMenuReferralCenterReminderTask(referralCenterMainMenuController), priority: MainMenuTasksPriorities.ReferralCenterReminderTask, "ReferralCenterReminderTask");
        mainMenuTaskChain.AddTask(new MainMenuShowSpecialOfferTask(mainMenuDynamicSpecialOfferController), priority: MainMenuTasksPriorities.DynamicSpecialOfferTask, "DynamicSpecialOfferTask");
        mainMenuTaskChain.AddTask(new MainMenuInstallPlayGamesTask(cloudSaveMessenger), priority: MainMenuTasksPriorities.InstallPlayGamesTask, "InstallPlayGamesTask");
        mainMenuTaskChain.AddTask(new MainMenuSavedGameAuthFailedTask(cloudSaveMessenger), priority: MainMenuTasksPriorities.AuthFailedTask, "AuthFailedTask");
        mainMenuTaskChain.AddTask(new MainMenuCloudDeviceConflictTask(cloudSaveMessenger), priority: MainMenuTasksPriorities.CloudDeviceConflictTask, "CloudDeviceConflictTask");
        mainMenuTaskChain.AddTask(new MainMenuSavedGameConflictTask(cloudSaveMessenger), priority: MainMenuTasksPriorities.SavedGameConflictTask, "SavedGameConflictTask");
        mainMenuTaskChain.AddTask(new MainMenuSkipScenarioTask(), priority: MainMenuTasksPriorities.SkipScenarioTask, "SkipScenarioTask");
        mainMenuTaskChain.AddTask(new MainMenuRateGameTask(), priority: MainMenuTasksPriorities.RateUsTask, "RateUsTask");

        if(!gameManager.IsLanguageChanging)
            mainMenuTaskChain.Execute(delegate {  }, delegate {  });
    }

    private void InitializePlayButton()
    {
        selectedLevelText.SetText((gameManager.profiler.LastUnlockedLevel + 1).ToString());
        if (!gameManager.IsPlayerFinishedMainCampaign)
        {
            var levelConfig = gameManager.levelManager.GetLevelConfig(gameManager.profiler.LastUnlockedLevel);
            if (levelConfig.HasChallengeLevel())
                playButtonImage.sprite = challengePlayButtonSprite;
            else
            {
                playButtonImage.sprite =
                    GetPlayButtonIconBasedOnLevelDifficulty(levelConfig.levelConfig.difficultyLevel);
            }
        }
        else
        {
            playButtonImage.sprite =
                GetPlayButtonIconBasedOnLevelDifficulty(DifficultyLevel.Normal);
        }
    }

    private Sprite GetPlayButtonIconBasedOnLevelDifficulty(DifficultyLevel difficultyLevel)
    {
        foreach (var playButtonSprite in playButtonSprites)
        {
            if (difficultyLevel == playButtonSprite.DifficultyLevel)
            {
                return playButtonSprite.playIconSprite;
            }
        }
        return null;
    }
    
    public void OnStarSectionClick()
    {
        AnalyticsManager.SendEvent(new AnalyticsData_Map_HomeTap("tap star icon"));
        gameManager.OpenPopup<Popup_GameMainLoopTutorial>();
    }

    public void OnStartClick()
    {
        gameManager.fxPresenter.PlayClickAudio();
        gameManager.tutorialManager.CheckAndHideTutorial(4);
        gameManager.tutorialManager.CheckAndHideTutorial(72);
        gameManager.tutorialManager.CheckAndHideTutorial(73);
        CheckStartPlay();
    }

    public void CheckStartPlay()
    {
        var levelInfoOpener = new LevelInfoOpener();
        levelInfoOpener.TryToOpenLevelInfo(UpdateGui);
    }

    public void OnSelectLevelClick()
    {
        gameManager.profiler.LastUnlockedLevel++;
        //gameManager.profiler.PlayCount = 0;
        if (gameManager.profiler.LastUnlockedLevel >= gameManager.levelManager.TotalLevels())
            gameManager.profiler.LastUnlockedLevel = 0;
        selectedLevelText.SetText((gameManager.profiler.LastUnlockedLevel + 1).ToString());
    }


    public void OnSettingsButtonClick()
    {
        gameManager.OpenPopup<Popup_Settings>().Setup(hudPresentationController);
        gameManager.fxPresenter.PlayClickAudio();
    }

    public void OnLifeSectionClick()
    {
        gameManager.fxPresenter.PlayClickAudio();
        var lifeManager = ServiceLocator.Find<ILife>();
        if (lifeManager.IsInInfiniteLife())
        {
            gameManager.OpenPopup<Popup_InfiniteLife>().Setup(() => { });
        }
        else
        {
            gameManager.OpenPopup<Popup_Life>().Setup(UpdateGui);
        }

    }

    public void OnCoinSectionClick()
    {
        gameManager.fxPresenter.PlayClickAudio();
        AnalyticsManager.SendEvent(new AnalyticsData_Map_HomeTap("tap coin icon"));
        gameManager.shopCreator.TrySetupMainShop("map", UpdateGui);
    }

    private void UpdateGui()
    {
        starCountText.SetText(gameManager.profiler.StarCount.ToString());
        coinCountText.SetText(gameManager.profiler.CoinCount.ToString());
    }

    public override void Back()
    {
        gameManager.OpenPopup<Popup_ExitMessage>().Setup((confirm) =>
        {
            if (confirm)
            {
                AnalyticsManager.SendEvent(new AnalyticsData_App_Close());
                Quit();
            }
        });

    }

    private static void Quit()
    {
        Application.Quit();
        //System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
    

    void AddInventory(string itemName, int value, string analyticsKey)
    {
        switch (itemName)
        {
            case "coin":
                gameManager.profiler.ChangeCoin(value, analyticsKey);
                break;

        }

        UpdateGui();
    }

    public void OnEvent(GameEvent evt, object sender)
    {
        if (evt is ServerConfigEvent serverConfig)
        {
            var serverConfigData = ServiceLocator.Find<ServerConfigManager>().data;
            // TODO : after 24h Resource Analytics sink/source is not handled
            if (serverConfig.serverConfigData.config.infiniteLifeReward.after24hLogin >= 0 && gameManager.tutorialManager.IsTutorialShowed(27) &&
              serverConfigData.serverTimeUtc > 0 && ((serverConfig.serverConfigData.serverTimeUtc - serverConfigData.serverTimeUtc) > 86400) && gameManager.CurrentPopup == this)
            { //Infinite life reward
                var lifeManager = ServiceLocator.Find<ILife>();
                lifeManager.AddInfiniteLifeSecond(Utilities.NowTimeUnix(), serverConfig.serverConfigData.config.infiniteLifeReward.after24hLogin);
                gameManager.OpenPopup<Popup_InfiniteLife>().Setup(() => { });
            }
        }
        else if (evt is SocialNetworkFreeCoinEvent data)
        {
            AddInventory("coin", data.amount, data.analyticsKey);
        }
        else if (evt is UpdateGUIEvent)
        {
            UpdateGui();
        }
    }


    #endregion

    [Serializable]
    public class ByDifficultyPlayButtonSprite
    {
        public DifficultyLevel DifficultyLevel;
        public Sprite playIconSprite;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        foreach (GameObject notifierParent in notifiersParents)
            notifierParent.SetActive(false);
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        foreach (GameObject notifierParent in notifiersParents)
            notifierParent.SetActive(true);
    }

    public override void PlayOpenAnimation()
    {
        expandEffectPlayer.PlayShowEffect();
        
        base.PlayOpenAnimation();
    }

    public override void PlayCloseAnimation()
    {
        expandEffectPlayer.PlayHideEffect();
        
        base.PlayCloseAnimation();
    }

    public override void StopRunningAnimation()
    {
        expandEffectPlayer.StopRunningEffect();
        
        base.StopRunningAnimation();
    }
}