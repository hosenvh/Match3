using System;
using Match3.Game.NeighborhoodChallenge;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using static GameAnalyticsDataProvider;
using static Match3.Overlay.Analytics.ResourcesAnalytics.ResourcesBaseAnalyticsHandler;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class ResourcesAnalyticsAdapter : AnalyticsAdapter
    {
        private ResourcesAnalyticsPortController portController;
        private ResourcesAnalyticsPortRecovery portRecovery;

        public ResourcesAnalyticsAdapter(UserProfileManager profileManager, GameProfiler userProfile, NeighborhoodChallengeManager neighborhoodChallengeManager, bool isFirstSession)
        {
            Initialize();
            AddHandlers(profileManager, userProfile, neighborhoodChallengeManager, isFirstSession);
            AddUserResourcesChangesListeners(neighborhoodChallengeManager);
            AddStatesOpeningClosingsListeners();
        }

        private void Initialize()
        {
            portController = new ResourcesAnalyticsPortController();
            portController.onOpenPortsChange += ResourcesAnalyticsLogger.UpdateOpenPortsStackTrace;

            portRecovery = new ResourcesAnalyticsPortRecovery(portController);
        }

        private void AddHandlers(UserProfileManager profileManager, GameProfiler userProfile, NeighborhoodChallengeManager neighborhoodChallengeManager, bool isFirstSession)
        {
            ResourceAnalyticsHandlerActions actions = new ResourceAnalyticsHandlerActions(onNeedToOpenPort: portController.OpenPort, onNeedToClosePort: portController.ClosePort);

            if (ShouldLetInitialResourcesToBeHandled())
                RegisterHandler(new InitialResourcesAnalyticsHandler(profileManager, userProfile, neighborhoodChallengeManager, isFirstSession, actions, onResourcesDataIsReadyToBeSent: portController.SendResourcesAnalytics));

            RegisterHandler(new WinPortResourceAnalyticsHandler(actions));
            RegisterHandler(new LosePortResourceAnalyticsHandler(actions));
            RegisterHandler(new MidLevelPortResourceAnalyticsHandler(actions));
            RegisterHandler(new ChallengeLevelPortResourceAnalyticsHandler(actions));

            RegisterHandler(new MainMenuPortResourceAnalyticsHandler(actions));
            RegisterHandler(new LobbyPortResourceAnalyticsHandler(actions));
            RegisterHandler(new DogTrainingPortResourceAnalyticsHandler(actions));
            RegisterHandler(new JokerPortResourceAnalyticsHandler(actions));
            RegisterHandler(new SeasonPassPortResourceAnalyticsHandler(actions));
            RegisterHandler(new ClanPortResourceAnalyticsHandler(actions));

            RegisterHandler(new EndOfTasksPortResourceAnalyticsHandler(actions));
            RegisterHandler(new LifePopupPortResourceAnalyticsHandler(actions));
            RegisterHandler(new SpinnerPortResourceAnalyticsHandler(actions));
            RegisterHandler(new LevelInfoPortResourceAnalyticsHandler(actions));

            RegisterHandler(new ShopPortResourceAnalyticsHandler(actions));
            RegisterHandler(new OtherShopsPortResourceAnalyticsHandler(actions));
            RegisterHandler(new MiscPortResourceAnalyticsHandler(actions));
        }

        private bool ShouldLetInitialResourcesToBeHandled()
        {
            return IsServerResourcesAnalyticsEnabled();
        }

        public static bool IsServerResourcesAnalyticsEnabled()
        {
            #if RESOURCES_ANALYTHICS_TEST
            return true;
            #endif
            ServerConfigData configData = ServiceLocator.Find<ServerConfigManager>().data;
            return configData != null && configData.config.analyticsEvents != null && configData.config.analyticsEvents.isResourcesEventsSendingEnable;
        }

        private void AddUserResourcesChangesListeners(NeighborhoodChallengeManager neighborhoodChallengeManager)
        {
            GameProfiler.OnCoinCountChange += (previousCount, currentCount) => portController.SendResourcesAnalytics(resourceCurrencyType: ResourcesCurrencyType.Coin, amount: currentCount - previousCount);
            LifeManager.OnLifeCountChange += (previousCount, currentCount) => portController.SendResourcesAnalytics(resourceCurrencyType: ResourcesCurrencyType.Life, amount: currentCount - previousCount);
            GameProfiler.OnStarCountChange += (previousCount, currentCount) => portController.SendResourcesAnalytics(resourceCurrencyType: ResourcesCurrencyType.Star, amount: currentCount - previousCount);
            GameProfiler.OnKeyCountChange += (previousCount, currentCount) => portController.SendResourcesAnalytics(resourceCurrencyType: ResourcesCurrencyType.Key, amount: currentCount - previousCount);
            neighborhoodChallengeManager.Ticket().OnValueChanged += (previousCount, currentCount) => portController.SendResourcesAnalytics(resourceCurrencyType: ResourcesCurrencyType.Ticket, amount: currentCount - previousCount);
            GameProfiler.OnPowerUpCountChange += (powerUpIndex, previousCount, currentCount) => portController.SendResourcesAnalytics(resourceCurrencyType: ResourcesAnalyticsUtility.ConvertPowerUpIndexToCurrencyType(powerUpIndex), currentCount - previousCount);
            Base.gameManager.profiler.BoosterManager.OnBoosterCountChange += (boosterIndex, previousCount, currentCount) => portController.SendResourcesAnalytics(resourceCurrencyType: ResourcesAnalyticsUtility.ConvertBoosterIndexToCurrencyType(boosterIndex), currentCount - previousCount);
            Base.gameManager.profiler.BoosterManager.OnInfiniteBoosterAdded += (boosterIndex, duration) => portController.SendResourcesAnalytics(resourceCurrencyType: ResourcesAnalyticsUtility.ConvertBoosterIndexToCurrencyType(boosterIndex), duration / 300f); // each 25 minutes means 5 booster
        }

        private void AddStatesOpeningClosingsListeners()
        {
            Base.gameManager.OnOpenState += state => DoForEachHandlerWithExceptionCatching(handler => handler.HandleGameStatesOpenings(state), occuredExceptionMessageDescription: "State Opening");
            Base.gameManager.OnCloseState += state => DoForEachHandlerWithExceptionCatching(handler => handler.HandleGameStatesClosings(state), occuredExceptionMessageDescription: "State Closing");
        }

        private void DoForEachHandlerWithExceptionCatching(Action<ResourcesBaseAnalyticsHandler> toDo, string occuredExceptionMessageDescription)
        {
            try
            {
                foreach (ResourcesBaseAnalyticsHandler resourcesBaseAnalyticsHandler in FindAllHandler<ResourcesBaseAnalyticsHandler>())
                    toDo.Invoke(resourcesBaseAnalyticsHandler);
            }
            catch (Exception e)
            {
                ResourcesAnalyticsLogger.LogException(e, occuredExceptionMessageDescription);
            }
        }

        public override void OnEvent(GameEvent evt, object sender)
        {
            try
            {
                base.OnEvent(evt, sender);
            }
            catch (Exception exception)
            {
                ResourcesAnalyticsLogger.LogException(exception, info: "On Event");
            }
        }
    }
}