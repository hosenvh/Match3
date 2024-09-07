using Match3.CloudSave;
using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.ReferralMarketing;
using Match3.Game.SkipScenario;
using Match3.LevelReachedReward.Game;
using Match3.LiveOps.CandyEvent.Game;
using Match3.LiveOps.Herbarium.Game;
using Match3.LiveOps.HerbariumCandyBase.Game;
using Match3.LuckySpinner.Base.Presentation;
using Match3.Overlay.Advertisement.Service.Session;
using Match3.Presentation.NeighborhoodChallenge;
using Match3.Presentation.UpdateWelcome;
using Match3.UserManagement.ProfileName.Presentation;
using SeganX;
using static GameAnalyticsDataProvider;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class MiscPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        private static class MiscItemId
        {
            public const string UpdateWelcome = "UpdateWelcome";
            public const string LevelReachedReward = "LevelReachedReward";
            public const string Ads = "Ads";
            public const string CloudService = "CloudService";
            public const string SkipScenario = "SkipScenario";
            public const string DynamicDifficulty = "DynamicDifficulty";
            public const string UserNameChange = "UsernameChange";
            public const string NeighbourhoodChangeLevel = "NeighbourChangeLevel";
            public const string GiftCode = "GiftCode";
            public const string Referral = "Referral";
            public const string FirebaseNotification = "Notif";
            public const string HerbariumLiveopsEvent = "Herbarium";
            public const string CandyLiveopsEvent = "Candy";
        }

        private static class PreAdsState
        {
            public const string LifePopup = "_LifePopup";
            public const string LevelLose = "_LevelLose";
            public const string LevelWin = "_LevelWin";
            public const string TaskPopup = "_TaskPopup";
            public const string MainMenu = "_MainMenu";
            public const string Spinner = "_Spinner";
        }

        private string preAdsState;

        public MiscPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(itemType: ResourcesItemType.Misc, actions)
        {
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_UpdateWelcome _:
                    OpenPort(itemId: GetUpdateWelcomeItemId());
                    break;
                case Popup_UserProfileNameRegistration _:
                    OpenPort(itemId: GetNeighbourhoodUserNameChangingItemId());
                    break;
            }
            HandlePreAdsState(gameState, true);
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
            switch (gameState)
            {
                case Popup_UpdateWelcome _:
                    ClosePort(itemId: GetUpdateWelcomeItemId());
                    break;
                case Popup_UserProfileNameRegistration _:
                    ClosePort(itemId: GetNeighbourhoodUserNameChangingItemId());
                    break;
            }
            HandlePreAdsState(gameState, isStateOpened: false);
        }

        protected override void Handle(GameEvent evt)
        {
            HandlePreAdsState(evt);

            HandleListeningToAds(evt);
            HandleListeningToCloudData(evt);
            HandleListeningToSkipScenario(evt);
            HandleListeningToDynamicDifficulty(evt);
            HandleListeningToNeighbourhoodLevelChanging(evt);
            HandleListeningToGiftCode(evt);
            HandleListeningToReferral(evt);
            HandleListeningToFirebaseNotificationRewardGiving(evt);
            HandleListeningToHerbariumOrCandy(evt);
            HandleListeningToLevelReachedReward(evt);
        }

        private void HandlePreAdsState(GameState gameState, bool isStateOpened)
        {
            switch (gameState)
            {
                case Popup_MainMenu _ when isStateOpened:
                    preAdsState = PreAdsState.MainMenu;
                    break;
                case Popup_Tasks _ when isStateOpened:
                    preAdsState = PreAdsState.TaskPopup;
                    break;
                case Popup_Life _ when isStateOpened:
                    preAdsState = PreAdsState.LifePopup;
                    break;
                case Popup_Life _ when !isStateOpened:
                    preAdsState = PreAdsState.MainMenu;
                    break;
                case Popup_Tasks _ when !isStateOpened:
                    preAdsState = PreAdsState.MainMenu;
                    break;
                case Popup_LuckySpinner _ when isStateOpened:
                    preAdsState = PreAdsState.Spinner;
                    break;
                case Popup_LuckySpinner _ when !isStateOpened:
                    preAdsState = PreAdsState.MainMenu;
                    break;
            }
        }

        private void HandlePreAdsState(GameEvent evt)
        {
            switch (evt)
            {
                case LevelEndedEvent result when result.result == LevelResult.Win:
                    preAdsState = PreAdsState.LevelWin;
                    break;
                case LevelEndedEvent result when result.result == LevelResult.Lose:
                    preAdsState = PreAdsState.LevelLose;
                    break;
            }
        }

        private void HandleListeningToAds(GameEvent evt)
        {
            switch (evt)
            {
                case AdvertisementSession.AdvertisementSessionOpenEvent _:
                    OpenPort(itemId: GetAdsItemId());
                    break;
                case AdvertisementSession.AdvertisementSessionCloseEvent _:
                    ClosePort(itemId: GetAdsItemId());
                    break;
            }
        }

        private void HandleListeningToCloudData(GameEvent evt)
        {
            switch (evt)
            {
                case GeneralDataHandler.CloudServiceDataLoadStartedEvent _:
                    OpenPort(itemId: GetCloudServiceDataItemId());
                    break;
                case GeneralDataHandler.CloudServiceDataLoadFinishedEvent _:
                    ClosePort(itemId: GetCloudServiceDataItemId());
                    break;
            }
        }

        private void HandleListeningToSkipScenario(GameEvent evt)
        {
            switch (evt)
            {
                case SkipScenarioStartedEvent _:
                    OpenPort(itemId: GetSkipScenarioItemId());
                    break;
                case SkipScenarioFinishedEvent _:
                    ClosePort(itemId: GetSkipScenarioItemId());
                    break;
            }
        }

        private void HandleListeningToDynamicDifficulty(GameEvent evt)
        {
            switch (evt)
            {
                case DynamicDifficultyRewardGivingStartedEvent _:
                    OpenPort(itemId: GetDynamicDifficultyItemId());
                    break;
                case DynamicDifficultyRewardGivingFinishedEvent _:
                    ClosePort(itemId: GetDynamicDifficultyItemId());
                    break;
            }
        }

        private void HandleListeningToNeighbourhoodLevelChanging(GameEvent evt)
        {
            switch (evt)
            {
                case NeighbourChangeLevelPopupOpenedEvent _:
                    OpenPort(itemId: GetNeighbourhoodLevelChangingItemId());
                    break;
                case NeighbourChangeLevelPopupClosedEvent _:
                    ClosePort(itemId: GetNeighbourhoodLevelChangingItemId());
                    break;
            }
        }

        private void HandleListeningToGiftCode(GameEvent evt)
        {
            switch (evt)
            {
                case GiftCodeRewardGivingStartedEvent _:
                    OpenPort(itemId: GetGiftCodeItemId());
                    break;
                case GiftCodeRewardGivingFinishedEvent _:
                    ClosePort(itemId: GetGiftCodeItemId());
                    break;
            }
        }

        private void HandleListeningToReferral(GameEvent evt)
        {
            switch (evt)
            {
                case ReferralRewardGivingStartedEvent _:
                    OpenPort(itemId: GetReferralRewardGivingItemId());
                    break;
                case ReferralRewardGivingFinishedEvent _:
                    ClosePort(itemId: GetReferralRewardGivingItemId());
                    break;
            }
        }

        private void HandleListeningToFirebaseNotificationRewardGiving(GameEvent evt)
        {
            switch (evt)
            {
                case FirebaseNotificationGiftHelper.FireBaseNotificationRewardGivingStartedEvent _:
                    OpenPort(itemId: GetFirebaseNotificationRewardGivingItemId());
                    break;
                case FirebaseNotificationGiftHelper.FireBaseNotificationRewardGivingFinishedEvent _:
                    ClosePort(itemId: GetFirebaseNotificationRewardGivingItemId());
                    break;
            }
        }

        private void HandleListeningToHerbariumOrCandy(GameEvent evt)
        {
            switch (evt)
            {
                case OnHerbariumCandyRewardGivingStarted rewardGivingStarted:
                    OpenPort(itemId: GetHerbariumCandyRewardGivingItemId(rewardGivingStarted));
                    break;
                case OnHerbariumCandyRewardGivingFinished rewardGivingFinished:
                    ClosePort(itemId: GetHerbariumCandyRewardGivingItemId(rewardGivingFinished));
                    break;
            }

        }

        private void HandleListeningToLevelReachedReward(GameEvent evt)
        {
            switch (evt)
            {
                case OnLevelReachedRewardGivingStarted _:
                    OpenPort(itemId: GetLevelReachedRewardItemId());
                    break;
                case OnLevelReachedRewardGivingFinished _:
                    ClosePort(itemId: GetLevelReachedRewardItemId());
                    break;
            }
        }

        private string GetAdsItemId()
        {
            return $"{MiscItemId.Ads}{preAdsState}";
        }

        private string GetUpdateWelcomeItemId()
        {
            return MiscItemId.UpdateWelcome;
        }

        private string GetLevelReachedRewardItemId()
        {
            return MiscItemId.LevelReachedReward;
        }

        private string GetCloudServiceDataItemId()
        {
            return MiscItemId.CloudService;
        }

        private string GetSkipScenarioItemId()
        {
            return MiscItemId.SkipScenario;
        }

        private string GetDynamicDifficultyItemId()
        {
            return MiscItemId.DynamicDifficulty;
        }

        private string GetNeighbourhoodUserNameChangingItemId()
        {
            return MiscItemId.UserNameChange;
        }

        private string GetNeighbourhoodLevelChangingItemId()
        {
            return MiscItemId.NeighbourhoodChangeLevel;
        }

        private string GetGiftCodeItemId()
        {
            return MiscItemId.GiftCode;
        }

        private string GetReferralRewardGivingItemId()
        {
            return MiscItemId.Referral;
        }

        private string GetFirebaseNotificationRewardGivingItemId()
        {
            return MiscItemId.FirebaseNotification;
        }

        private string GetHerbariumCandyRewardGivingItemId(OnHerbariumCandyRewardGivingBase rewardGivingEvent)
        {
            switch (rewardGivingEvent.eventGeneralName)
            {
                case HerbariumEventController.EVENT_GENERAL_NAME:
                    return GetHerbariumRewardGivingItemId();
                case CandyLiveOpsEventController.EVENT_GENERAL_NAME:
                    return GetCandyRewardGivingItemId();
                default: return "Not_Defined_HerbariumCandy";
            }
        }

        private string GetHerbariumRewardGivingItemId()
        {
            return MiscItemId.HerbariumLiveopsEvent;
        }

        private string GetCandyRewardGivingItemId()
        {
            return MiscItemId.HerbariumLiveopsEvent;
        }
    }
}