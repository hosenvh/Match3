using System;
using Match3.Data.Unity.PersistentTypes;
using Match3.Foundation.Base.ServiceLocating;
using Match3.LiveOps.Foundation;
using Match3.LiveOps.SeasonPass.Game;
using Medrick.LiveOps.Foundation;
using UnityEngine;


namespace Match3.CloudSave
{
    public class SeasonPassDataHandler : ICloudDataHandler
    {
        private const string totalBadgeCountKeyPostfix = "EARNED_BADGES_COUNT";
        private const string hasGoldenTicketKeyPostfix = "IS_GOLDEN_TICKET_PURCHASED";

        private string seasonPassSharedKey;

        private string SeasonPassSharedKey
        {
            get
            {
                if (seasonPassSharedKey.IsNullOrEmpty())
                {
                    EventLifeTimeManager lifeTimeManager = ServiceLocator.Find<GolmoradLiveOpsManager>().lifeTimeManager;
                    var eventRoot = lifeTimeManager.GetAllEventRoots().Find(root => root.GetType() == typeof(SeasonPassEventRoot));
                    if (eventRoot == null)
                        return string.Empty;
                    var seasonPassEventRoot = (SeasonPassEventRoot) eventRoot;
                    seasonPassSharedKey = $"{SeasonPassEventRoot.EVENT_GENERAL_NAME}_{seasonPassEventRoot.MetaData.GetId()}";
                }
                return seasonPassSharedKey;
            }
        }


        public void CollectData(ICloudDataStorage cloudStorage)
        {
            string totalBadgeCountKey = GetTotalBadgesKey();
            PersistentInt totalBadgesCount = new PersistentInt(totalBadgeCountKey);
            try
            {
                cloudStorage.SetInt(totalBadgeCountKey, totalBadgesCount.Get(0));
            }
            catch (Exception e)
            {
                Debug.LogError($"Collecting Failed {totalBadgeCountKey} - {e}");
            }

            string hasGoldenTicketKey = GetGoldenTicketKey();
            PersistentBool hasGoldenTicket = new PersistentBool(hasGoldenTicketKey);
            try
            {
                cloudStorage.SetInt(hasGoldenTicketKey, hasGoldenTicket.Get(false) ? 1 : 0);
            }
            catch (Exception e)
            {
                Debug.LogError($"Collecting Failed {hasGoldenTicketKey} - {e}");
            }

            for (int i = 0; i < 100; i++)
            {
                string freeKey = GetFreeRewardClaimedKey(i);
                string premiumKey = GetPremiumRewardClaimedKey(i);
                PersistentBool isFreeClaimed = new PersistentBool(key: freeKey);
                PersistentBool isPremiumClaimed = new PersistentBool(key: premiumKey);
                try
                {
                    cloudStorage.SetInt(freeKey, isFreeClaimed.Get(false) ? 1 : 0);
                    cloudStorage.SetInt(premiumKey, isPremiumClaimed.Get(false) ? 1 : 0);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Collecting Failed {freeKey} - {e}");
                }
            }

            string finalPrizeKey = GetFinalPrizeClaimedKey();
            PersistentBool isFinalPrizeClaimed = new PersistentBool(finalPrizeKey);
            try
            {
                cloudStorage.SetInt(finalPrizeKey, isFinalPrizeClaimed.Get(false) ? 1 : 0);

            }
            catch (Exception e)
            {
                Debug.LogError($"Collecting Failed {finalPrizeKey} - {e}");
            }
        }

        public void SpreadData(ICloudDataStorage cloudStorage)
        {
            string totalBadgeCountKey = GetTotalBadgesKey();
            PersistentInt totalBadgesCount = new PersistentInt(totalBadgeCountKey);
            try
            {
                totalBadgesCount.Set(cloudStorage.GetInt(totalBadgeCountKey));
            }
            catch (Exception e)
            {
                Debug.LogError($"Spreading Failed {totalBadgeCountKey} - {e}");
            }

            string hasGoldenTicketKey = GetGoldenTicketKey();
            PersistentBool hasGoldenTicket = new PersistentBool(hasGoldenTicketKey);
            try
            {
                hasGoldenTicket.Set(cloudStorage.GetInt(hasGoldenTicketKey, defaultValue: 0) == 1);
            }
            catch (Exception e)
            {
                Debug.LogError($"Spreading Failed {hasGoldenTicketKey} - {e}");
            }

            for (int i = 0; i < 100; i++)
            {
                string freeKey = GetFreeRewardClaimedKey(i);
                string premiumKey = GetPremiumRewardClaimedKey(i);
                PersistentBool isFreeClaimed = new PersistentBool(key: freeKey);
                PersistentBool isPremiumClaimed = new PersistentBool(key: premiumKey);


                try
                {
                    isFreeClaimed.Set(cloudStorage.GetInt(freeKey, defaultValue: 0) == 1);
                    isPremiumClaimed.Set(cloudStorage.GetInt(premiumKey, defaultValue: 0) == 1);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Spreading Failed {freeKey} - {e}");
                }
            }

            string finalPrizeKey = GetFinalPrizeClaimedKey();
            PersistentBool isFinalPrizeClaimed = new PersistentBool(finalPrizeKey);
            try
            {
                isFinalPrizeClaimed.Set(cloudStorage.GetInt(finalPrizeKey, 0) == 1);
            }
            catch (Exception e)
            {
                Debug.LogError($"Spreading Failed {isFinalPrizeClaimed} - {e}");
            }
        }

        private string GetTotalBadgesKey()
        {
            return $"{SeasonPassSharedKey}_{totalBadgeCountKeyPostfix}";
        }

        private string GetGoldenTicketKey()
        {
            return $"{SeasonPassSharedKey}_{hasGoldenTicketKeyPostfix}";
        }

        private string GetFreeRewardClaimedKey(int index)
        {
            return $"{SeasonPassSharedKey}_REWARD_BUNDLE_FREE_{index}_IS_CLAIMED";
        }

        private string GetPremiumRewardClaimedKey(int index)
        {
            return $"{SeasonPassSharedKey}_REWARD_BUNDLE_PREMIUM_{index}_IS_CLAIMED";
        }

        private string GetFinalPrizeClaimedKey()
        {
            return $"{SeasonPassSharedKey}_REWARD_BUNDLE_FINAL_PRIZE_IS_CLAIMED";
        }
    }
}