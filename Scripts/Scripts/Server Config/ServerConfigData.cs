using System.Collections.Generic;
using CloudSave.Data.configuration;
using Match3.Clan.Data;
using Match3.Data.Configuration;
using Match3.Data.ShopManagement;
using Match3.Game.NewsSpace;
using Match3.LevelReachedReward.Data;
using Match3.LiveOps.Foundation;
using Match3.LuckySpinner.CoinBased.Data;
using Match3.LuckySpinner.TimeBased.Data;
using Match3.Main.VideoPlayer;
using Match3.Overlay.Advertisement.Placements.Data;
using Match3.Overlay.Advertisement.Players.Data;
using Match3.Overlay.Advertisement.Players.Implementations;
using Match3.Scenario;
using Match3.ServerCommunication.Main.JWT.Data;
using Match3.ThirdParties.Zarinpal;
using Match3.UserManagement.ProfilePage.UserAvatarGallery.Data;
using Match3.Wardrobe.Data;

namespace Match3
{
    [System.Serializable]
    public class ServerConfigData
    {

        #region InnerClasses

        [System.Serializable]
        public class Links
        {
            public string market = "";
            public string instagram = "";
            public string telegram = "";
        }

        [System.Serializable]
        public class Config
        {
            public ChartboostExtraSettingsData chartboostExtraSettingsServerConfig = null;
            public AdvertisementPlayersDataContainer advertisementPlayersDataContainer_V3 = null;
            public AdvertisementPlacementsConfigurer advertisementPlacementsConfig;

            public MarketInitializerConfig marketServerConfig;
            public DynamicSpecialOfferConfig dynamicSpecialOfferConfig;
            public ShopPackagesPresentStage ShopPackagesPresentStage;
            public ShopCenteralConfig shopCenteralConfig;
            public HillaPayServerConfig hillaPayServerConfig;
            public ZarinpalServerConfig zarinpalServerConfig;
            public PurchasePossibility purchasePossibility;
            public bool isPaymentVerification = true;

            public FaultyBehaviourDetection faultyBehaviourDetection = null;
            public AnalyticsEvents analyticsEvents = null;

            public ServerLevelConfig[] serverLevelsConfig;
            public DynamicDifficulty dynamicDifficulty = null;

            public string playGamesDownloadLink;
            public string merchShopLink;
            public bool isCloudSaveActive = true;
            public CloudSaveServerConfig cloudSaveServiceConfig;
            public NeighborhoodChallengeServerConfig neighborhoodChallengeServerConfig;
            public IntroVideoPlayerServerConfig introVideoPlayerServerConfig;
            public ScenarioDialoguesAudioServerConfig scenarioDialoguesAudioServerConfig;
            public GameOverServerConfigData gameOverServerConfigData;
            public JWTServerSettings jwtServerSettingsConfig;
            public ClanServerConfig clanServerConfig;
            public NotificationServerConfigData notificationServerConfigData;

            public WardrobeServerConfigData wardrobeServerConfigData;
            public UserAvatarGalleryServerConfigData userAvatarGalleryServerConfigData;

            public CoinBasedLuckySpinnerServerConfig coinBasedLuckySpinnerServerConfig;
            public TimeBasedLuckySpinnerServerConfig timeBasedLuckySpinnerServerConfig;

            public InfiniteLifeReward infiniteLifeReward = null;
            public DailyRewardServerConfigData dailyRewardServerConfigData;
            public LevelReachedRewardConfig levelReachedRewardConfig;

            public Survey survey;
            public News[] newses = new News[0];
        }

        [System.Serializable]
        public class CohortConfig
        {
            public AdvertisementPlacementsConfigurer advertisementPlacementsConfig;

            public DynamicSpecialOfferConfig dynamicSpecialOfferConfig;
            public ShopPackagesPresentStage ShopPackagesPresentStage;
            public ShopCenteralCohortConfig shopCenteralCohortConfig;

            public CloudSaveCohortConfig cloudSaveCohortConfig;
            public NeighborhoodChallengeServerCohortConfig neighborhoodChallengeServerCohortConfig;
            public IntroVideoPlayerServerCohortConfig introVideoPlayerServerCohortConfig;
            public ScenarioDialoguesAudioServerCohortConfig scenarioDialoguesAudioServerCohortConfig;
            public GameOverServerCohortConfig gameOverServerCohortConfig;
            public ClanServerCohortConfig clanServerCohortConfig;
            public NotificationServerCohortConfig notificationServerCohortConfig;
            
            public WardrobeServerCohortConfig wardrobeServerCohortConfig;

            public LiveOpsEventMetaDataCohortConfig liveOpsEventMetaDataCohortConfig;
        }

        [System.Serializable]
        public class ShopPackagesPresentStage
        {
            public bool _isDataValid = false;
            public int[] firstStagePackages;
        }

        [System.Serializable]
        public class PurchasePossibility
        {
            public bool shouldCheckTimeZone = true;
        }

        [System.Serializable]
        public class AnalyticsEvents
        {
            public bool isResourcesEventsSendingEnable = false;
        }

        [System.Serializable]
        public class FaultyBehaviourDetection
        {
            public bool checkPaymentFaultyBehaviour = true;
        }

        [System.Serializable]
        public class DynamicDifficulty
        {
            [System.Serializable]
            public class DynamicDifficultyReward
            {
                public int neededPassedTimeSeconds;
                public int infiniteLifeRewardDuration;
                public float rainbowFactorCoefficient;
            }

            public int neededLooseCount;
            public List<DynamicDifficultyReward> rewards = new List<DynamicDifficultyReward>();
        }

        [System.Serializable]
        public class InfiniteLifeReward
        {
            public int after24hLogin = -1;
            public int endOfDay = 900;
        }

        [System.Serializable]
        public class Survey
        {
            public string link;
        }
        #endregion

        public string status;
        public string country;
        public string cohort;
        public long serverTimeUtc;
        public bool test;
        public bool debug;

        public Links links = new Links();
        public Config config = new Config();
        public CohortConfig cohortConfig = new CohortConfig();

    }
}
