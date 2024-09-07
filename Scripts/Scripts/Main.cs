using System;
using System.Globalization;
using Match3.CloudSave;
using Match3.CloudSave.Events;
using KitchenParadise.Foundation.Unity.PlatformFunctionality;
using Match3.CharacterManagement.Character.Game;
using Match3.CharacterManagement.CharacterSkin.Game;
using Match3.CharacterManagement.CharacterSkin.Overlay;
using Match3.Development.Base.DevelopmentConsole;
using Match3.Overlay.Advertisement.Placements;
using Match3.Foundation.Base.CohortManagement;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.FactorySystem;
using Match3.Foundation.Base.NotificationService;
using Match3.Foundation.Base.PlayerStats;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.TaskManagement;
using Match3.Foundation.Unity.Configuration;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game.DynamicDifficulty;
using Match3.Game.ReferralMarketing;
using Match3.Game.TaskManagement;
using Match3.LiveOps.Foundation;
using Match3.LiveOps.Main;
using Match3.Main.VideoPlayer;
using Match3.Network;
using Match3.Presentation;
using Match3.Presentation.MainMenu;
using Match3.ServerCommunication.Main;
using Match3.UserManagement.Avatar.Overlay;
using Match3.UserManagement.Main;
using Medrick.Foundation.Base.PlatformFunctionality;
using Match3.Utility.GolmoradLogging;
using Match3.Utility.GolmoradLogging.Config;
using UnityEngine;
using UnityEngine.Events;


namespace Match3.Main
{
    public class Main : MonoBehaviour
    {

        public UnityConfigurationManager configurationManager;
        public DevelopmentToolManager developmentToolManager;
        public GameObject primaryServicesContainer;
        public GolmoradLogsActivenessConfig logsActivenessConfig;

        public UnityEvent onPrimaryServicesInitialized;

        // Todo: Because of we haven't player stats saver in prev versions so we should still keep this for now! Remove later
        // and use currentSession instead
        public bool IsFirstTimePlay
        {
            get => PlayerPrefs.GetInt("_FirstTimePlay", 0) == 0;
            set => PlayerPrefs.SetInt("_FirstTimePlay", value ? 0 : 1);
        }

        public void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            SetCultureAsInvariant();
            Init();
        }

        private void SetCultureAsInvariant()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        }

        void Init()
        {
            DebugPro.Setup(logsActivenessConfig);

            ServiceLocator.Init();

            configurationManager.Init();

            ServiceLocator.Register(configurationManager);

            developmentToolManager.Init();

            RegisterServicesIn(primaryServicesContainer);

            ServiceLocator.Register(new MainMenuTaskChainService());
            
            ServiceLocator.Register(new AndroidPlatformFunctionalityManager());
            ServiceLocator.Register(new BasicEventManager());
            ServiceLocator.Register(new UserProfileManager());

            ServiceLocator.Register(new UserCohortAssignmentManager(new GenericServiceFactory<CohortManagementServerCommunicator>().Create()));

            ServiceLocator.Register(new GenericServiceFactory<ServerConnection>().Create());
            ServiceLocator.Register(new MainGameTransitionManager());
            ServiceLocator.Register(new AdvertisementPlacementsManager(ServiceLocator.Find<EventManager>(), configurationManager));

            ServiceLocator.Register(Configure(new PresentationElementActivationStateCenter()));

            ServiceLocator.Register(new CharactersManagerFactory().CreateCharactersManager());

            // Warning: This service has a very trivial implementation, do not use without refactoring and a complete implementation
            CreatePlayerStatsService();

            onPrimaryServicesInitialized.Invoke();

            CreateNotificationService();

            ServiceLocator.Register(new ReferralCenter());
            CreateAndSetSplashScreenTasks();

            InitializeIntroVideo();

            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");

            IsFirstTimePlay = false;
        }

        void InitSecondaryService()
        {
            try
            {
                ServerConfigManager serverConfigManager = new ServerConfigManager(ServiceLocator.Find<AndroidPlatformFunctionalityManager>());
                ServiceLocator.Register(serverConfigManager);
                InitCloudSaveServices();
                StoreAndMarketInitializer.Initialize(serverConfigManager.data.config.marketServerConfig);
                ShopInitializer.Initilize(serverConfigManager, ServiceLocator.Find<IMarketManager>());
                ServiceLocator.Register(new TimeManager());
                ServiceLocator.Register(new CharactersSkinsManagerFactory().CreateCharactersSkinsManager());
                ServiceLocator.Register(new UserManagementService());
                ServiceLocator.Register(new DynamicDifficultyManager(ServiceLocator.Find<UserProfileManager>(), ServiceLocator.Find<EventManager>(), serverConfigManager.data.config.dynamicDifficulty));
                ServiceLocator.Register(new MainMenuButtonBoard());

                ServiceLocator.Find<AdvertisementPlacementsManager>().Initialize();
                PurchasePossibilityChecker.Initialize(ServiceLocator.Find<IMarketManager>(), serverConfigManager.data);

                GolmoradLiveOpsCreator.Create(
                    configurationManager,
                    ServiceLocator.Find<IUserProfile>(),
                    ServiceLocator.Find<IMarketManager>(),
                    ServiceLocator.Find<PlatformFunctionalityManager>(),
                    ServiceLocator.Find<UnityTimeScheduler>(),
                    serverConfigManager);

                InitCloudSaveServices();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
            }
        }

        private void CreateAndSetSplashScreenTasks()
        {
            var splashScreenTaskChain = new SequentialTaskChain();
            
            splashScreenTaskChain.AddTask(new LanguageSelectionTask(IsFirstTimePlay));
            splashScreenTaskChain.AddTask(new GenericAsyncTask(ServiceLocator.Find<UserProfileManager>().Init));
            splashScreenTaskChain.AddTask(new GenericSyncTask(InitSecondaryService));
            splashScreenTaskChain.AddTask(Configure(new GolmoradServerCommunicationInitializationTask()));
            splashScreenTaskChain.AddTask(new GenericAsyncTask(ServiceLocator.Find<UserCohortAssignmentManager>().ManageUserCohort));
            splashScreenTaskChain.AddTask(new ServerConfigUpdatingTask()); // TODO: Refactor this section so that this server config updating task can be the first, currently if this results in a broken/wrong server config, it may cause exceptions to the `InitSecondaryService` task, and the game may stuck on the splash screen
            splashScreenTaskChain.AddTask(new HillaPayMarketSetupTask());
            splashScreenTaskChain.AddTask(new ZarinpalMarketSetupTask());
            splashScreenTaskChain.AddTask(new GolmoradLiveOpsInitializationTask());
            splashScreenTaskChain.AddTask(new ReferralCenterInitializationTask());
            splashScreenTaskChain.AddTask(new CloudSaveServiceTask(IsFirstTimePlay));
            splashScreenTaskChain.AddTask(new FirebaseInitializationTask());

            State_Loading.taskChain = splashScreenTaskChain;
        }

        private void CreatePlayerStatsService()
        {
            var playerStatService = new PlayerStatsService();
            ServiceLocator.Register(playerStatService);
            playerStatService.OpenNewSession();
        }

        private void RegisterServicesIn(GameObject container)
        {
            var services = container.transform.GetComponentsInChildren<Service>();
            foreach (var service in services)
                    ServiceLocator.Register(service);
        }

        private void CreateNotificationService()
        {
            var notificationDataStorage = new NotificationServiceDataStorage();
            var notificationService = new NotificationService(notificationDataStorage);
            configurationManager.Configure(notificationService);
            ServiceLocator.Register(notificationService);
        }
       
        private void InitCloudSaveServices()
        {
            var serverConfigManager = ServiceLocator.Find<ServerConfigManager>();
            var cloudService = new CloudSaveService(false);
            cloudService.Setup(serverConfigManager.data.config.cloudSaveServiceConfig.cloudSaveManagerType,
                               serverConfigManager.data.config.cloudSaveServiceConfig.cloudSaveImplementationControllerType);
            ServiceLocator.Register(cloudService);

            var characterDataHandler = new CharactersDataHandler();
            var generalDataHandler = new GeneralDataHandler();
            var mapItemDataHandler = new MapItemDataHandler();
            var mapManagerDataHandler = new MapManagerDataHandler();
            var paidMapItemDataHandler = new PaidMapItemDataHandler();
            var taskManagerDataHandler = new TaskManagerDataHandler();
            var userInfoDataHandler = new UserInfoDataHandler();
            var tutorialDataHandler = new TutorialDataHandler();
            var miscDataHandler = new MiscDataHandler();
            var ncDataHandler = new NCDataHandler();
            var seasonPassDataHandler = new SeasonPassDataHandler();
            var characterSkinsDataHandler = new CharacterSkinsCloudDataHandler();
            var userAvatarsDataHandler = new UserAvatarsCloudDataHandler();

            var dataHandlers = new ICloudDataHandler[]
            {
                characterDataHandler, generalDataHandler, mapItemDataHandler, mapManagerDataHandler,
                paidMapItemDataHandler, taskManagerDataHandler, userInfoDataHandler, tutorialDataHandler, miscDataHandler, 
                ncDataHandler, seasonPassDataHandler, characterSkinsDataHandler, userAvatarsDataHandler
            };
            cloudService.AddDataHandlers(dataHandlers);
            
            var eventManager = ServiceLocator.Find<EventManager>();
            ServiceLocator.Register(new PlayerSaveBackupService(dataHandlers));
            ServiceLocator.Register(new CloudSaveAutomaticSavingService(eventManager, cloudService));
        }

        private void InitializeIntroVideo()
        {
            if (!IsFirstTimePlay) return;
            IntroVideoMapTask.introVideoPlayer = new IntroVideoPlayer();
        }
        
        private T Configure<T>(T entity)
        {
            configurationManager.Configure(entity);
            return entity;
        }
    }
}