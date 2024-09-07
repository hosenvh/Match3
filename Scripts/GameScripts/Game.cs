using System.Collections.Generic;
using Bundles.PopupTransitionEffects.Scripts;
using DG.Tweening;
using DynamicSpecialOfferSpace;
using KitchenParadise.Foundation.Base.TimeManagement;
using KitchenParadise.Utiltiy.Base;
using Match3;
using Match3.Clan.Game;
using Match3.Clan.Overlay.Analytics;
using Match3.DailyReward.Game;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.FaultyBehaviourDetection;
using Match3.Game.Gameplay;
using Match3.Game.Map;
using Match3.Game.NeighborhoodChallenge;
using Match3.Game.NewsSpace;
using Match3.Game.PiggyBank;
using Match3.Game.RainBowCountReseter;
using Match3.Game.ShopManagement;
using Match3.Game.SkipScenario;
using Match3.Game.SocialAlbum;
using Match3.Game.UpdateWelcome;
using Match3.LevelReachedReward.Game;
using Match3.LuckySpinner.MainMenuLuckySpinnerManager.Game;
using Match3.Main;
using Match3.Main.NeighborhoodChallenge;
using Match3.Presentation.Gameplay;
using Match3.Presentation.NeighborhoodChallenge;
using Match3.Presentation.ShopManagement;
using Match3.Presentation.TransitionEffects;
using Match3.Profiler;
using Match3.ServerCommunication.Main;
using Match3.UserManagement.Foundation.Base;
using Match3.UserManagement.ProfilePage.UserAvatarGallery.Game;
using Match3.Utility.GolmoradLogging;
using Match3.Wardrobe.Game;
using Medrick.Clan;
using Medrick.Foundation.ShopManagement.Core;
using Medrick.ServerCommunication.Basic.Application.Response;
using Match3.Game.Inbox;
using Newtonsoft.Json;
using NotificationSpace;
using SeganX;
using UnityEngine;
using UnityEngine.Events;


public class GameOpenEvent: GameEvent {}

public class Game : GameManager
{
    public interface NonGameplayPresentationPort
    {
    }

    public int skipLevel;
    public GameObject services;
    public TaskManager taskManager; // todo : it should be on State_Map
    public GameProfiler profiler;
    public LevelSessionProfiler levelSessionProfiler;
    public LevelManager levelManager;
    public TutorialManager tutorialManager;
    public FxPresenter fxPresenter; // todo : it should be gameFxPresenter and mapFxPresenter and it should be loaded only if state_game (or state_map) is open
    public MusicManager musicManager;
    public JoySystem joySystem;
    public SkipScenarioController skipScenarioController;
    public MapItemManager mapItemManager;
    public ShopPopupCreator shopCreator;
    public MapManager mapManager;
    public ScenarioManager scenarioManager;
    public MapCharactersManager mapCharactersManager;
    public SocialAlbumController socialAlbumController;
    private DailyRewardController dailyRewardController;
    public WardrobeController wardrobeController;
    public UserAvatarGalleryController userAvatarGalleryController;
    public LevelReachedRewardController levelReachedRewardController;
    public ClanController clanController;
    public ClanViewController clanViewController;
    private ClanAnalyticsController clanAnalyticsController;

    public PopupTransitionEffectController popupTransitionEffectController;


    public UnityEvent onServiceInitialized;

    BasicUpdateManager updateManager;
    public Channel mainTimeChannel = new IDedChannel(0);

    public DynamicSpecialOfferManager dynamicSpecialOfferManager;
    public UpdateWelcomeController updateWelcomeController;

    public FirebaseNotificationSubscribeController firebaseNotificationSubscribeController;

    public bool IsLanguageChanging { get; private set; }
    public Transform presentationHandlersContainer;

    private LuckySpinnerMainMenuManager luckySpinnerMainMenuManager;
    private List<NonGameplayPresentationPort> presentationPorts = new List<NonGameplayPresentationPort>();

    public bool IsPlayerFinishedMainCampaign => profiler.LastUnlockedLevel >= levelManager.LastPublishedLevelIndex();

    public static bool IsInMainCampaign => gameManager != null && gameManager.CurrentState != null &&
                                           (gameManager.CurrentState is CampaignGameplayState ||
                                            gameManager.CurrentState is State_Map);

    public static bool IsInNeighborhoodChallenge => gameManager != null && gameManager.CurrentState != null &&
                                             (gameManager.CurrentState is NeighborhoodChallengeGameplayState ||
                                              gameManager.CurrentState is State_NeighborhoodChallengeLobby);


    private const string IsFirstOpenString = "isFirstOpen";
    bool IsFirstOpen
    {
        get { return PlayerPrefs.GetInt(IsFirstOpenString, 1) == 1; }
        set { PlayerPrefs.SetInt(IsFirstOpenString, value ? 1 : 0); }
    }

    private void Awake()
    {
        Resources.UnloadUnusedAssets();
        Application.targetFrameRate = 60;
        DOTween.Init();
        AddAllPresentationPorts();
        InitGameServices();
        InitNonLiveUpsGameEvents();
        InitOverlays(IsFirstOpen);

        firebaseNotificationSubscribeController = new FirebaseNotificationSubscribeController();
        levelSessionProfiler = new LevelSessionProfiler();

        var configurationManager = ServiceLocator.Find<ConfigurationManager>();
        configurationManager.Configure(taskManager);
        configurationManager.Configure(levelManager);

        NeighborhoodChallengeInitializer.Initialize();

        updateManager = ServiceLocator.Find<BasicUpdateManager>();
        updateManager.RegisterChannel(mainTimeChannel);

        updateWelcomeController = new UpdateWelcomeController(IsFirstOpen);

        InitializeDynamicSpecialOffer();

        AnalyticsManager.SendEvent(new AnalyticsData_App_Open());
        if (IsFirstOpen)
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Flag_First_Open());
            IsFirstOpen = false;
            //DelayCall(.2f, SkipLevels);
        }

        ServiceLocator.Find<EventManager>().Propagate(new GameOpenEvent(), this);

        InitSkipScenario();

        shopCreator = new ShopPopupCreator(
            ServiceLocator.Find<GolmoradShopCenter>(),
            ServiceLocator.Find<IMarketManager>(),
            this);

        scenarioManager.Setup();

        InitWronglyGivenRainbowReseter(configurationManager);
        InitLuckySpinnerMainMenuManager(configurationManager);
        InitDailyReward();
        InitWardrobe();
        InitUserAvatarGallery();
        InitLevelReachedReward();
        InitClan();
        InitPopupTransitionEffect();

        DebugPro.LogInfo<UserLogTag>($"Device id: {SystemInfo.deviceUniqueIdentifier} ");
        DebugPro.LogInfo<UserLogTag>($"User id: {ServiceLocator.Find<UserProfileManager>().GlobalUserId} ");
    }

    private void InitDailyReward()
    {
        dailyRewardController = new DailyRewardController();
    }

    private void InitWardrobe()
    {
        wardrobeController = new WardrobeController();
    }

    private void InitUserAvatarGallery()
    {
        userAvatarGalleryController = new UserAvatarGalleryController();
    }

    private void InitLevelReachedReward()
    {
        levelReachedRewardController = new LevelReachedRewardControllerFactory().CreateController();
    }

    private void InitClan()
    {
        if (ShouldActivateClan() == false)
            return;

        var serverRequestHandler = ServiceLocator.Find<GolmoradServerCommunicationService>().ServerRequestHandler;

        clanController = new ClanController(serverRequestHandler);
        clanViewController = new ClanViewController(clanController);
        clanAnalyticsController = new ClanAnalyticsController();

        bool ShouldActivateClan() => ServiceLocator.Find<ServerConfigManager>().data.config.clanServerConfig.IsClanActive;
    }

    private void InitPopupTransitionEffect()
    {
        popupTransitionEffectController = new PopupTransitionEffectController();
    }

    private void AddAllPresentationPorts()
    {
        foreach(var handler in presentationHandlersContainer.GetComponentsInChildren<NonGameplayPresentationPort>())
            presentationPorts.Add(handler);
    }

    public T GetPresentationPort<T>() where T : NonGameplayPresentationPort
    {
        foreach (NonGameplayPresentationPort ports in presentationPorts)
            if (ports is T port)
                return port;

        return default;
    }

    private void InitWronglyGivenRainbowReseter(ConfigurationManager configurationManager)
    {
        var rainbowCountResetter = new RainbowCountResetter(taskManager);
        configurationManager.Configure(rainbowCountResetter);
        rainbowCountResetter.Start();
    }

    private void InitLuckySpinnerMainMenuManager(ConfigurationManager configurationManager)
    {
        luckySpinnerMainMenuManager = new LuckySpinnerMainMenuManager();
        configurationManager.Configure(luckySpinnerMainMenuManager);
    }

    private void InitGameServices()
    {
        ServiceLocator.Register(new BasicUpdateManager());
        ServiceLocator.Register(new MainCellFactory());
        ServiceLocator.Register(new MainTileFactory());
        ServiceLocator.Register(new MainCellAtttachmentFactory());
        ServiceLocator.Register(new RainbowFillValueBalanceManager());

        ServiceLocator.Register(new DataManager());
        // TODO: Move this to overlays.
        ServiceLocator.Register(new GameplayTutorialManager());

        ServiceLocator.Register(new LifeManager(5));
        ServiceLocator.Register(new MapItemPurchaseManager());

        ServiceLocator.Register(new NeighborhoodChallengeManager());
        ServiceLocator.Register(new NeighborhoodChallengeTutorialManager(this.tutorialManager));

        ServiceLocator.Register(new NewsManager());
        ServiceLocator.Register(new FaultyBehaviourDetectionService(
            ServiceLocator.Find<EventManager>(),
            ServiceLocator.Find<UserProfileManager>(),
            ServiceLocator.Find<ITimeManager>(),
            gameManager.profiler,
            IsFirstOpen));

        ServiceLocator.Register(new InboxManager());
        ServiceLocator.Register(new NotificationCentre());
        InitPiggyBankManager();

        services.GetComponentsInChildren<Service>().ForEach(s => ServiceLocator.Register(s));


        ServiceLocator.Find<ShopCenter>().OnPurchaseSucceeded += (package, _) => profiler.TrackPurchase(package as HardCurrencyPackage);

        onServiceInitialized.Invoke();
    }

    private void InitNonLiveUpsGameEvents()
    {
    }

    private void InitOverlays(bool isFirstSession)
    {
        new AnalyticsInitializer().Initialize(isFirstSession, ServiceLocator.Find<ShopCenter>());
    }

    private void InitSkipScenario()
    {
        skipScenarioController = new SkipScenarioController();
        ServiceLocator.Find<ConfigurationManager>().Configure(skipScenarioController);
    }

    private void InitPiggyBankManager()
    {
        var piggyBankManager = new PiggyBankManager(ServiceLocator.Find<GolmoradShopCenter>());
        ServiceLocator.Register(piggyBankManager);
    }

    private void InitializeDynamicSpecialOffer()
    {
        var serverConfig = ServiceLocator.Find<ServerConfigManager>().data.config;
        dynamicSpecialOfferManager = new DynamicSpecialOfferManager(
            serverConfig.dynamicSpecialOfferConfig,
            ServiceLocator.Find<ShopCenter>(),
            profiler);
    }

    void Start()
    {
        ServiceLocator.Find<GameTransitionManager>().GoToLastMap<EmptyTransitionEffect>();

        DelayCall(2f, () =>
        {
            AnalyticsManager.SendEvent(new AnalyticsData_App_Load());
        });
    }

    private void OnApplicationFocus(bool focus)
    {
        ServiceLocator.Find<EventManager>().Propagate(new AppExit(),new AppExit()
        {
            Focus = focus
        });
        
        if (!focus)
        {
            PlayerPrefs.Save();
            AnalyticsManager.SendEvent(new AnalyticsData_App_Minimize(mapManager.IsInMap() ? page_names.main_menu : page_names.match));
        }
        else {
            ServiceLocator.Find<ITimeManager>().MarkTimeNotSyncedWithServer();
            var serverConfigRequest = new ServerConfigRequest();
            serverConfigRequest.UpdateServerTime((ret) => { });
        }
    }

    private void OnApplicationPause(bool pause)
    {
        PlayerPrefs.Save();
    }

    private void Update()
    {
        updateManager.AdvanceTime(Time.deltaTime);
    }

    public void SetLanguageChanging(bool isChanging)
    {
        IsLanguageChanging = isChanging;
    }

    public DailyRewardManager Debug_GetDailyRewardEventController()
    {
        return dailyRewardController.Debug_GetDailyRewardEventManager();
    }

    #if UNITY_EDITOR
    [ContextMenu("Skip Levels")]
    private void SkipLevels()
    {
        for (int i = 0; i < skipLevel; i++)
        {
            TaskConfig taskConfig = null;
            foreach (var item in taskManager.CurrentTasksList)
            {
                taskConfig = item;
                profiler.SetStarCount(profiler.StarCount - item.requiremnetStars);
                if(item.id > 1)
                    profiler.LastUnlockedLevel += item.requiremnetStars;
                break;
            }
            taskManager.SetTaskDone(taskConfig);
            List<ScenarioItem> scenarioItems = taskManager.GetScenarioItems(taskConfig);
            gameManager.scenarioManager.SaveScenarioStates(scenarioItems);
        }
    }

#endif
}