using Match3.Clan.Presentation.Popups;
using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelContinuing;
using Match3.Game.NeighborhoodChallenge;
using Match3.Game.ShopManagement;
using Match3.LiveOps.DogTraining.Presentation.Lobby;
using Match3.LiveOps.Joker.Presentation;
using Match3.LuckySpinner.Base.Presentation;
using Match3.Presentation.NeighborhoodChallenge;
using Match3.UserManagement.ProfileName.Presentation;
using SeganX;
using static GameAnalyticsDataProvider;
using static Match3.Overlay.Analytics.ResourcesAnalytics.ResourcesAnalyticsUtility;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class ShopPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        private static class PlayerPrePurchaseState
        {
            public const string LifePopup = "LifePopup_" + DEFAULT_ITEM_ID + "_";
            public const string TicketPopup = "TicketPopup_" + DEFAULT_ITEM_ID + "_";
            public const string ChangeUserName = "UserNameChange_" + DEFAULT_ITEM_ID + "_";
            public const string NeighbourChangeLevel = "NeighbourChangeLevel_" + DEFAULT_ITEM_ID + "_";
            public const string LevelInfo = "LevelInfo_";
            public const string LevelInfoRetry = "LevelInfoRetry_";
            public const string Lobby = "Lobby_" + DEFAULT_ITEM_ID + "_";
            public const string DogTrainingLobby = "DogTraining_" + DEFAULT_ITEM_ID + "_";
            public const string Joker = "Joker_" + DEFAULT_ITEM_ID + "_";
            public const string MainMenu = "MainMenu_" + DEFAULT_ITEM_ID + "_";
            public const string MiddleOfLevel = "MidLevel_";
            public const string EndOfLevel = "EndGame_";
            public const string LuckySpinner = "LuckySpinner_";
            public const string Clan = "Clan_";
        }

        private readonly LevelInfoPortResourceAnalyticsHandler levelInfoPortResourceAnalyticsHandler;

        private string prePurchaseState;
        private string prePrePurchaseState;
        private string preShopActiveLevelType;
        private string preShopGoalLevelType;
        private bool isNeighbourhoodChangeLevelPending;

        private string PrePurchaseState
        {
            get => prePurchaseState;
            set
            {
                prePrePurchaseState = PrePurchaseState;
                prePurchaseState = value;
            }
        }

        public ShopPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(ResourcesItemType.Shop, actions)
        {
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_MainMenu _:
                    HandleMainMenuOpened();
                    break;
                case State_NeighborhoodChallengeLobby _:
                    HandleNeighbourhoodLobbyOpened();
                    break;
                case State_DogTrainingLobby _:
                    HandleDogTrainingLobbyOpened();
                    break;
                case Popup_JokerMain _:
                    HandleJokerPopupOpened();
                    break;
                case Popup_Life _:
                case Popup_InfiniteLife _:
                    HandleLifePopupOpened();
                    break;
                case Popup_TicketRefill _:
                    HandleTicketPopupOpened();
                    break;
                case Popup_UserProfileNameRegistration _:
                    HandleUserNameChangePopupOpened();
                    break;
                case Popup_LevelInfo _:
                    HandleLevelInfoOpened();
                    break;
                case Popup_LuckySpinner _:
                    HandleLuckySpinnerOpened();
                    break;
                case Popup_ClanMain _:
                    HandleClanOpened();
                    break;
            }
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_Life _:
                case Popup_InfiniteLife _:
                    HandleLifePopupClosed();
                    break;
                case Popup_TicketRefill _:
                    HandleTicketPopupClosed();
                    break;
                case Popup_UserProfileNameRegistration _:
                    HandleUserNameChangePopupClosed();
                    break;
                case Popup_Shop _ when isNeighbourhoodChangeLevelPending:
                    HandleNeighbourChangeLevelPopupClosed();
                    break;
                case Popup_JokerMain _:
                    HandleJokerPopupClosed();
                    break;
                case Popup_LevelInfo _:
                    HandleLevelInfoClosed();
                    break;
                case Popup_LuckySpinner _:
                    HandleLuckySpinnerClosed();
                    break;
                case Popup_ClanMain _:
                    HandleLuckySpinnerClosed();
                    break;
            }
        }

        protected override void Handle(GameEvent evt)
        {
            switch (evt)
            {
                case ShopPurchaseResultGivingStartedEvent data:
                    OpenPort(itemId: GetCorrespondingItemIdFromStorePackageData(data.package));
                    break;
                case SocialNetworkFreeRewardGivingStartedEvent data:
                    OpenPort(itemId: GetCorrespondingItemIdFromSocialName(data.socialName));
                    break;

                case ShopPurchaseResultGivingFinishedEvent data:
                    ClosePort(itemId: GetCorrespondingItemIdFromStorePackageData(data.package));
                    break;
                case SocialNetworkFreeRewardGivingFinishedEvent data:
                    ClosePort(itemId: GetCorrespondingItemIdFromSocialName(data.socialName));
                    break;

                case LevelResumeWithExtraMoveEvent _:
                case LevelResumeWithAdsExtraMoveEvent _:
                case LevelStartedEvent _:
                    TryToHandleLevelStarted();
                    break;
                case LevelEndedEvent _:
                    TryToHandleLevelEnded();
                    break;

                case NeighbourChangeLevelPopupOpenedEvent _:
                    HandleNeighbourChangeLevelPopupOpened();
                    break;
                case NeighbourChangeLevelPopupClosedEvent data:
                    if (data.confirmResult == false)
                        HandleNeighbourChangeLevelPopupClosed();
                    break;
                case NCLevelChangedEvent _:
                    HandleNeighbourChangeLevelPopupClosed();
                    break;
            }
        }

        private void TryToHandleLevelStarted()
        {
            if (CanLevelBeStarted())
                HandleLevelStarted();
            else
                LogErrorCantStartLevel();
        }

        private void TryToHandleLevelEnded()
        {
            if (CanLevelBeEnded())
                HandleLevelEnded();
            else
                LogErrorCantEndLevel();
        }

        private void HandleLevelStarted()
        {
            PrePurchaseState = PlayerPrePurchaseState.MiddleOfLevel;
            preShopActiveLevelType = GetCurrentLevelType();
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleLevelEnded()
        {
            PrePurchaseState = PlayerPrePurchaseState.EndOfLevel;
            preShopActiveLevelType = GetCurrentLevelType();
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleMainMenuOpened()
        {
            PrePurchaseState = PlayerPrePurchaseState.MainMenu;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleNeighbourhoodLobbyOpened()
        {
            PrePurchaseState = PlayerPrePurchaseState.Lobby;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleDogTrainingLobbyOpened()
        {
            PrePurchaseState = PlayerPrePurchaseState.DogTrainingLobby;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleJokerPopupOpened()
        {
            PrePurchaseState = PlayerPrePurchaseState.Joker;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleJokerPopupClosed()
        {
            PrePurchaseState = prePrePurchaseState;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleLifePopupOpened()
        {
            PrePurchaseState = PlayerPrePurchaseState.LifePopup;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleLifePopupClosed()
        {
            PrePurchaseState = prePrePurchaseState;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleTicketPopupOpened()
        {
            PrePurchaseState = PlayerPrePurchaseState.TicketPopup;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleTicketPopupClosed()
        {
            PrePurchaseState = prePrePurchaseState;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleUserNameChangePopupOpened()
        {
            PrePurchaseState = PlayerPrePurchaseState.ChangeUserName;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleUserNameChangePopupClosed()
        {
            PrePurchaseState = prePrePurchaseState;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleNeighbourChangeLevelPopupOpened()
        {
            isNeighbourhoodChangeLevelPending = true;
            PrePurchaseState = PlayerPrePurchaseState.NeighbourChangeLevel;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleNeighbourChangeLevelPopupClosed()
        {
            isNeighbourhoodChangeLevelPending = false;
            PrePurchaseState = prePrePurchaseState;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleLevelInfoOpened()
        {
            PrePurchaseState = LevelInfoPortResourceAnalyticsHandler.IsLevelInfoRetryMode() ? PlayerPrePurchaseState.LevelInfoRetry : PlayerPrePurchaseState.LevelInfo;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelInfoPortResourceAnalyticsHandler.GetLevelInfoTargetLevelType();
        }

        private void HandleLuckySpinnerOpened()
        {
            PrePurchaseState = PlayerPrePurchaseState.LuckySpinner;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleClanOpened()
        {
            PrePurchaseState = PlayerPrePurchaseState.Clan;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleLevelInfoClosed()
        {
            PrePurchaseState = prePrePurchaseState;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleLuckySpinnerClosed()
        {
            PrePurchaseState = prePrePurchaseState;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private void HandleClanClosed()
        {
            PrePurchaseState = prePrePurchaseState;
            preShopActiveLevelType = LevelType.Null;
            preShopGoalLevelType = LevelType.Null;
        }

        private bool CanLevelBeStarted()
        {
            return PrePurchaseState == PlayerPrePurchaseState.MainMenu || PrePurchaseState == PlayerPrePurchaseState.Lobby || PrePurchaseState == PlayerPrePurchaseState.DogTrainingLobby || PrePurchaseState == PlayerPrePurchaseState.EndOfLevel;
        }

        private bool CanLevelBeEnded()
        {
            return PrePurchaseState == PlayerPrePurchaseState.MiddleOfLevel;
        }

        private string GetCorrespondingItemIdFromStorePackageData(HardCurrencyPackage package)
        {
            return $"{GetCorrespondingItemIdBase()}{package.GetCamelCaseSku()}";
        }

        private string GetCorrespondingItemIdFromSocialName(string socialName)
        {
            return $"{GetCorrespondingItemIdBase()}{socialName.Replace("_", string.Empty)}";
        }

        private string GetCorrespondingItemIdBase()
        {
            return $"{PrePurchaseState}{preShopActiveLevelType}{preShopGoalLevelType}";
        }

        private void LogErrorCantStartLevel()
        {
            LogError("Start Level Event Listening Bug");
        }

        private void LogErrorCantEndLevel()
        {
            LogError("End Level Event Listening Bug");
        }

        private void LogError(string baseMessage)
        {
            ResourcesAnalyticsLogger.LogError($"{baseMessage}. Current PrePurchaseState: {prePurchaseState}, PreShopActiveLevelType: {preShopActiveLevelType}, PreShopGoalLevelType: {preShopGoalLevelType}");
        }
    }
}