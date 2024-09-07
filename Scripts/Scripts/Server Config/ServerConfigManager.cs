using KitchenParadise.Foundation.Unity.PlatformFunctionality;
using Match3.Overlay.Advertisement.Placements;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using System;
using Match3.Foundation.Base.EventManagement;
using UnityEngine;

namespace Match3
{
    public class ServerConfigManager : Service
    {
        public event Action<ServerConfigData> onServerConfigUpdated = delegate { };

        public ServerConfigData data = null;

        AndroidPlatformFunctionalityManager androidFunctionalityManager;

        public ServerConfigManager(AndroidPlatformFunctionalityManager androidFunctionalityManager)
        {
            this.androidFunctionalityManager = androidFunctionalityManager;
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
        }

        public void Init(ServerConfigData defaultData)
        {
            if (!IsPlayerGotAtLeastOneServerData())
            {
                data = defaultData;
                // This is done to not modify the defaultData
                Save();
                data = new ServerConfigData();
                Load();

                Restore(data);
                ClearUpdateDataVersion();
            }
            else
            {
                data = new ServerConfigData();
                Load();
                
                Restore(data);
            }
        }

        private void ApplyCohortConfigOn(ServerConfigData data)
        {
            if (data.cohortConfig == null)
                return;

            var validator = new ServerConfigDataValidator();
            
            if (validator.IsDynamicSpecialOfferConfigValid(data.cohortConfig.dynamicSpecialOfferConfig))
                data.config.dynamicSpecialOfferConfig = data.cohortConfig.dynamicSpecialOfferConfig;

            if (data.cohortConfig.advertisementPlacementsConfig != null && data.cohortConfig.advertisementPlacementsConfig._isDataValid)
                data.config.advertisementPlacementsConfig = data.cohortConfig.advertisementPlacementsConfig;

            if (data.cohortConfig.ShopPackagesPresentStage != null 
                && data.cohortConfig.ShopPackagesPresentStage._isDataValid)
                data.config.ShopPackagesPresentStage = data.cohortConfig.ShopPackagesPresentStage;

            data.cohortConfig.shopCenteralCohortConfig.Configure(ref data.config.shopCenteralConfig);
            data.cohortConfig.neighborhoodChallengeServerCohortConfig.Configure(ref data.config.neighborhoodChallengeServerConfig);
            data.cohortConfig.cloudSaveCohortConfig.Configure(ref data.config.cloudSaveServiceConfig);
            data.cohortConfig.introVideoPlayerServerCohortConfig.Configure(ref data.config.introVideoPlayerServerConfig);
            data.cohortConfig.scenarioDialoguesAudioServerCohortConfig.Configure(ref data.config.scenarioDialoguesAudioServerConfig);
            data.cohortConfig.wardrobeServerCohortConfig.Configure(ref data.config.wardrobeServerConfigData);
            data.cohortConfig.gameOverServerCohortConfig.Configure(ref data.config.gameOverServerConfigData);
            data.cohortConfig.clanServerCohortConfig.Configure(ref data.config.clanServerConfig);
            data.cohortConfig.notificationServerCohortConfig.Configure(ref data.config.notificationServerConfigData);
        }

        public void Update(ServerConfigData data)
        {
            Overwrite(data, isDataFresh: true);
        }

        public void Restore(ServerConfigData data)
        {
            Overwrite(data, isDataFresh: false);
        }

        private void Overwrite(ServerConfigData data, bool isDataFresh)
        {
            this.data = data;
            ApplyCohortConfigOn(data);

            HandleConfigurers(data.config);

            if (ServiceLocator.Has<ITimeManager>() && isDataFresh)
                ServiceLocator.Find<ITimeManager>().RefreshTimeSyncWithServer(data.serverTimeUtc);

            ServiceLocator.Find<EventManager>().Propagate(new ServerConfigEvent() { serverConfigData = data }, null);
            
            Save();

            onServerConfigUpdated.Invoke(data);
        }

        public void UpdateServerTime(long time)
        {
            data.serverTimeUtc = time;
            SaveServerTime();
        }

        // NOTE: It is better to register configurer in configurationManager, but in some cases (e.g. AdvertismentPlacementManager)
        // it may be need to reconfigure the targets (it's better to find a better mechanism for this).
        // NOTE: Because some configurers are registered from server config, their targets must be initialized after 
        // ServerConfigManager (or the target must be reconfigured).
        private void HandleConfigurers(ServerConfigData.Config config)
        {
            var configManager = ServiceLocator.Find<ConfigurationManager>();

            configManager.RegisterAdditiveOrReplace(config.neighborhoodChallengeServerConfig);

            config.advertisementPlacementsConfig.Configure(ServiceLocator.Find<AdvertisementPlacementsManager>());
        }

        //Refactor this code
        private void Save()
        {
            PlayerPrefs.SetInt("UpdateData_debug", (data.debug) ? 1 : 0);
            PlayerPrefs.SetInt("UpdateData_test", (data.test) ? 1 : 0);
            PlayerPrefs.SetString("UpdateData_status", data.status);
            PlayerPrefs.SetString("UpdateData_country", data.country);
            PlayerPrefs.SetString("UpdateData_cohort", data.cohort);
            PlayerPrefs.SetString("serverTimeKey", data.serverTimeUtc.ToString());
            PlayerPrefs.SetString("UpdateData_links_instagram", data.links.instagram);
            PlayerPrefs.SetString("UpdateData_links_telegram", data.links.telegram);
            PlayerPrefs.SetString("UpdateData_links_market", data.links.market);
            PlayerPrefs.SetString("config_serverData", JsonUtility.ToJson(data.config));
            PlayerPrefs.SetString("cohortConfig_serverData", JsonUtility.ToJson(data.cohortConfig));
            PlayerPrefs.SetInt("UpdateDataAppVersion", androidFunctionalityManager.VersionCode());
        }
        
        private void SaveServerTime()
        {
            PlayerPrefs.SetString("serverTimeKey", data.serverTimeUtc.ToString());
        }
        

        private void Load()
        {
            data.debug = PlayerPrefs.GetInt("UpdateData_debug", 0) == 1;
            data.test = PlayerPrefs.GetInt("UpdateData_test", 0) == 1;
            data.status = PlayerPrefs.GetString("UpdateData_status", "OK");
            data.country = PlayerPrefs.GetString("UpdateData_country", "");
            data.cohort = PlayerPrefs.GetString("UpdateData_cohort", "");
            data.serverTimeUtc = long.Parse(PlayerPrefs.GetString("serverTimeKey", "0"));
            data.links = new ServerConfigData.Links();
            data.links.instagram = PlayerPrefs.GetString("UpdateData_links_instagram", "instagram://user?username=bazigolmorad");
            data.links.telegram = PlayerPrefs.GetString("UpdateData_links_telegram", "https://t.me/BaziGolmorad");
            data.links.market = PlayerPrefs.GetString("UpdateData_links_market", "");
            data.config = JsonUtility.FromJson<ServerConfigData.Config>(PlayerPrefs.GetString("config_serverData", "{}"));
            data.cohortConfig = JsonUtility.FromJson<ServerConfigData.CohortConfig>(PlayerPrefs.GetString("cohortConfig_serverData", "{}"));
        }

        public int GetInfiniteLifeRewardEndOfDay()
        {
            if (data.config.infiniteLifeReward == null)
                return 900;
            return data.config.infiniteLifeReward.endOfDay;
        }

        public bool IsPlayerGotAtLeastOneServerData()
        {
            // NOTE: The first condition may be redundant
            return PlayerPrefs.HasKey("UpdateData_cohort") && 
                PlayerPrefs.GetInt("UpdateDataAppVersion", 0) >= androidFunctionalityManager.VersionCode();
        }

        private void ClearUpdateDataVersion()
        {
            PlayerPrefs.SetInt("UpdateDataAppVersion", 0);
        }
    }
}