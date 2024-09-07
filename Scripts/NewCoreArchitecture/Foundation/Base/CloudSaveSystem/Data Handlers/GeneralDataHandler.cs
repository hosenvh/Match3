using Match3;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;



namespace Match3.CloudSave
{

    public class GeneralDataHandler : ICloudDataHandler
    {
        public class CloudServiceDataLoadStartedEvent : GameEvent {}
        public class CloudServiceDataLoadFinishedEvent : GameEvent {}

        private const string BoosterKey = "booster_";
        private const string PowerUpKey = "powerup_";
        private const string CoinKey    = "coin";
        private const string LifeKey    = "life";
        private const string LastGetLifeTimeKey    = "lastGetLifeTime";
        private const string StarKey    = "star";
        private const string KeyItemKey = "key" ;
        private const string LastUnlockedLevelKey = "lastUnlockedLevel";
        private const string PrivacyPolicyCheckKey = "privacyPolicyCheck";
        private const string TelegramFollowRewardKey = "telegramReward";
        private const string InstagramFollowRewardKey = "instagramReward";
        private const string ServeyRewardKey = "surveyReward";
        private const string PurchaseCountKey = "purchaseCount";


        public void CollectData(ICloudDataStorage cloudStorage)
        {
            var profiler = Base.gameManager.profiler;
            var userProfileManager = ServiceLocator.Find<IUserProfile>();
            
            cloudStorage.SetInt(CoinKey, profiler.CoinCount);
            cloudStorage.SetInt(StarKey, profiler.StarCount);
            cloudStorage.SetInt(KeyItemKey, profiler.KeyCount);
            cloudStorage.SetInt(LastUnlockedLevelKey, profiler.LastUnlockedLevel);
            cloudStorage.SetInt(PrivacyPolicyCheckKey, profiler.IsPrivacyPolicyChecked?1:0);
            
            cloudStorage.SetInt(LifeKey, profiler.LifeCount);
            cloudStorage.SetString(LastGetLifeTimeKey, profiler.lastGetLifeTime.ToString());

            cloudStorage.SetInt(PurchaseCountKey, profiler.PurchaseCount);

            cloudStorage.SetInt(TelegramFollowRewardKey, userProfileManager.LoadData<int>(Popup_Shop.telegramFreeCoinKey, 0));
            cloudStorage.SetInt(InstagramFollowRewardKey, userProfileManager.LoadData<int>(Popup_Shop.instagramFreeCoinKey, 0));
            cloudStorage.SetInt(ServeyRewardKey, userProfileManager.LoadData<int>(Popup_Shop.surveyFreeCoinKey, 0));


            for (int i = 0; i <= 2; ++i)
            {
                cloudStorage.SetInt($"{BoosterKey}{i}", profiler.GetBoosterCount(i));
                cloudStorage.SetInt($"{PowerUpKey}{i}", profiler.GetPowerupCount(i));
            }
        }

        
        public void SpreadData(ICloudDataStorage cloudStorage)
        {
            var profiler = Base.gameManager.profiler;
            var userProfileManager = ServiceLocator.Find<IUserProfile>();
            var eventManager = ServiceLocator.Find<EventManager>();

            eventManager.Propagate(new CloudServiceDataLoadStartedEvent(), this);

            profiler.SetCoinCount(cloudStorage.GetInt(CoinKey));
            profiler.SetStarCount(cloudStorage.GetInt(StarKey));
            profiler.SetKeyCount(cloudStorage.GetInt(KeyItemKey));
            profiler.SetLastUnlockedLevel(cloudStorage.GetInt(LastUnlockedLevelKey));
            profiler.IsPrivacyPolicyChecked = cloudStorage.GetInt(PrivacyPolicyCheckKey) == 1;
            
            profiler.SetLifeCount(cloudStorage.GetInt(LifeKey));
            profiler.SetLastGetLifeTime(cloudStorage.GetString(LastGetLifeTimeKey));

            profiler.PurchaseCount = cloudStorage.GetInt(PurchaseCountKey, 0);

            userProfileManager.SaveData(Popup_Shop.telegramFreeCoinKey, cloudStorage.GetInt(TelegramFollowRewardKey));
            userProfileManager.SaveData(Popup_Shop.instagramFreeCoinKey, cloudStorage.GetInt(InstagramFollowRewardKey));
            userProfileManager.SaveData(Popup_Shop.surveyFreeCoinKey, cloudStorage.GetInt(ServeyRewardKey, 0));


            for (int i = 0; i <= 2; ++i)
            {
                profiler.BoosterManager.SetBoosterCount(i, cloudStorage.GetInt($"{BoosterKey}{i}"));
                profiler.SetPowerUpCount(i, cloudStorage.GetInt($"{PowerUpKey}{i}"));
            }

            eventManager.Propagate(new CloudServiceDataLoadFinishedEvent(), this);
        }
        
        
        
    }

}


