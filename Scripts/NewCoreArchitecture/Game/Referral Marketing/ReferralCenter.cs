using System;
using System.Collections;
using System.Linq;
using Match3.Data.ReferralMarketing;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.TaskManagement;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game.ReferralMarketing.Segments;
using NewCoreArchitecture.Game.Referral_Marketing;
using UnityEngine;


namespace Match3.Game.ReferralMarketing
{
    
    public enum ReferralGoalPrizeStatus
    {
        NotReach,
        NotClaimed,
        Claimed
    }

    public class ReferralCenterInitializedEvent : GameEvent {}
    
    public class ReferralCenterUnlockedEvent : GameEvent {}
    
    public class ReferralCenter : Service
    {
        private const string LAST_REFERRED_PLAYER_COUNT_KEY = "ReferralMarketing_LastReferredPlayerCount";
        
        public const int MaximumCurrentLevelToConsiderReferredPlayer = 10;
    
        public ReferralCenterServerController ServerController { get; private set; }
        public ReferralCenterShareSegmentController SegmentController { get; private set; }
        
        
        public int MaxReferralGoal { get; private set; }
        public string ReferralCode { get; private set; }
        public bool IsInitialized  { get; private set; } = false;
        public bool IsUnlocked { get; private set; }
        public bool IsReferralCodeUsed { get; private set; } = false;
        public int AutoUpdateInterval { get; private set; }
        public ReferralPrizeScriptableData[] ScriptablePrizes { get; private set; }
        public ReferredPlayer[] ReferredPlayers => initializeData.referredPlayers;
        
        
        private ReferralCenterUnlocker unlocker;

        private bool initializing = false;
        private bool updating = false;
        
        private Action OnInitializationComplete;
        private Action<InitializeFailureReason> OnInitializationFailed;
        
        private Action OnUpdatingComplete;
        private Action<InitializeFailureReason> OnUpdatingFailed;
        
        private Action OnHasUnclaimedReward;
        
        private InitializeData initializeData;

        private bool autoUpdate = false;


        // ================================================================================================ \\
        
        
        public ReferralCenter()
        {
            ServerController = new ReferralCenterServerController(this);
            SegmentController = new ReferralCenterShareSegmentController(this);
            
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
            
            unlocker = new ReferralCenterUnlocker(Unlock);
        }

        public void SetMaxReferralGoal(int maxReferralGoal)
        {
            MaxReferralGoal = maxReferralGoal;
        }

        public void SetScriptablePrizes(ReferralPrizeScriptableData[] prizes)
        {
            ScriptablePrizes = prizes;
        }

        public void SetAutoUpdateIntervalTime(int intervalTime)
        {
            AutoUpdateInterval = intervalTime;
        }

        public void SetShareSegments(ShareSegment[] shareSegments)
        {
            SegmentController.SetShareSegments(shareSegments);
        }

        public bool IsAvailable()
        {
            return IsInitialized && IsUnlocked;
        }
        
        public void Initialize(Action onSucceed, Action<InitializeFailureReason> onFailure)
        {
            if (IsInitialized)
            {
                onSucceed();
                return;
            }

            if (OnInitializationComplete == null || !OnInitializationComplete.GetInvocationList().Contains(onSucceed))
                OnInitializationComplete += onSucceed;
            if(OnInitializationFailed == null || !OnInitializationFailed.GetInvocationList().Contains(onFailure))
                OnInitializationFailed += onFailure;

            if (initializing)
                return;

            initializing = true;
            ServerController.GetInitializeData(
                initData =>
            {
                initializeData = initData;
                ReferralCode = initData.myReferralCode;
                IsReferralCodeUsed = !string.IsNullOrEmpty(initData.referralCodeInvitedWith);
                if(initData.goalPrizes.Length>0) OverridePrizesData(initData.goalPrizes);
                
                if (initializeData.referredPlayers.Length > 0 && GetLastReferredPlayerCount() == 0 && !HasUnclaimedReward())
                {
                    SetLastReferredPlayerCount(initializeData.referredPlayers.Length);
                }

                RestoreReferralMapItems();
                
                IsInitialized = true;

                ServiceLocator.Find<EventManager>().Propagate(new ReferralCenterInitializedEvent(), this);
                
                OnInitializationComplete.Invoke();
                OnInitializationComplete = null;
                OnInitializationFailed = null;
                
                initializing = false;
            }
                , failureReason =>
            {
                OnInitializationFailed.Invoke(failureReason);
                OnInitializationFailed = null;
                OnInitializationComplete = null;
                
                    initializing = false;
                });

            void RestoreReferralMapItems()
            {
                var mainMenuTaskChain = ServiceLocator.Find<MainMenuTaskChainService>();
                var task = new ReferralCenterMainMenuTask(onExecute: ApplyClaimedMapItemRewards);
                mainMenuTaskChain.AddTask(
                    task,
                    priority: MainMenuTasksPriorities.ReferralMapStateItemRestoreTask,
                    id: $"Referral_Map_Items");
            }

            void ApplyClaimedMapItemRewards()
            {
                foreach (var claimedIndex in initializeData.claimedGoalPrizes)
                    foreach (var referralReward in ScriptablePrizes[claimedIndex - 1].rewards)
                        if (referralReward.GetReward() is MapItemReward mapItemReward)
                            mapItemReward.Apply();
            }
        }

        public float Progress()
        {
            return (float)(initializeData.referredPlayers.Length - 1) / (float)(MaxReferralGoal - 1);
        }

        public void OnHasProgressSubscribe(Action onHasProgress)
        {
            OnHasUnclaimedReward += onHasProgress;
        }
        
        public void OnHasProgressUnsubscribe(Action onHasProgress)
        {
            OnHasUnclaimedReward -= onHasProgress;
        }

        public bool HasUnclaimedReward()
        {
            if (!IsInitialized) return false;

            foreach (var scriptablePrize in ScriptablePrizes)
            {
                if (GetReferralGoalPrizeStatus(scriptablePrize.goalId) == ReferralGoalPrizeStatus.NotClaimed)
                    return true;
            }

            return false;
        }
        
        public void UpdateReferralCenter(Action onSucceed, Action<InitializeFailureReason> onFailure)
        {
            if(OnUpdatingComplete == null || !OnUpdatingComplete.GetInvocationList().Contains(onSucceed))
                OnUpdatingComplete += onSucceed;
            if(OnUpdatingFailed == null || !OnUpdatingFailed.GetInvocationList().Contains(onFailure))
                OnUpdatingFailed += onFailure;

            if (updating) return;
            updating = true;
            
            ServerController.GetInitializeData(
                newData =>
                {
                    initializeData.referredPlayers = newData.referredPlayers;
                    initializeData.claimedGoalPrizes = newData.claimedGoalPrizes;
                    OnUpdatingComplete?.Invoke();
                    OnUpdatingComplete = null;
                    OnUpdatingFailed = null;
                    updating = false;
                }
                , reason =>
                {
                    OnUpdatingFailed?.Invoke(reason);
                    OnUpdatingFailed = null;
                    OnUpdatingComplete = null;
                    updating = false;
                });
        }
        
        public void StartAutoUpdating(Action onHasUnclaimedReward)
        {
            OnHasUnclaimedReward += onHasUnclaimedReward;
            
            if (autoUpdate) return;
            
            autoUpdate = true;
            ServiceLocator.Find<UnityTimeScheduler>()
                .StartCoroutine(UpdateReferralCenter(new WaitForSeconds(AutoUpdateInterval)));
        }

        public void StopAutoUpdating()
        {
            autoUpdate = false;
        }


        public string GetSegmentNameByTag(string tag)
        {
            foreach (var shareSegment in SegmentController.ShareSegments)
            {
                if (shareSegment.tag == tag)
                {
                    return shareSegment.GetType().Name;
                }
            }

            return "None";
        }

        public ReferralGoalPrizeStatus GetReferralGoalPrizeStatus(int goalId)
        {
            if (initializeData.claimedGoalPrizes.Contains(goalId)) return ReferralGoalPrizeStatus.Claimed;

            var referredPlayersCount = initializeData.referredPlayers.Length;
            
            foreach (var scriptablePrize in ScriptablePrizes)
            {
                if (scriptablePrize.goalId == goalId)
                {
                    if (referredPlayersCount >= scriptablePrize.referredPlayerRequire)
                        return ReferralGoalPrizeStatus.NotClaimed;
                }
            }
            
            return ReferralGoalPrizeStatus.NotReach;
        }

        public ReferralReward GetGoalReferralReward(int goalId)
        {
            foreach (var scriptablePrize in ScriptablePrizes)
            {
                if (scriptablePrize.goalId == goalId)
                    return scriptablePrize.GetReferralReward();
            }

            return null;
        }
        
        
        
        public void SetReferralCodeUsed()
        {
            IsReferralCodeUsed = true;
        }

        public void SetGoalPrizeClaimed(int goalId)
        {
            if (initializeData.claimedGoalPrizes.Contains(goalId)) return;
            var claimedRewards = initializeData.claimedGoalPrizes.ToList();
            claimedRewards.Add(goalId);
            claimedRewards.Sort();
            initializeData.claimedGoalPrizes = claimedRewards.ToArray();
        }


        public bool HasNewReferredPlayer()
        {
            return initializeData.referredPlayers.Length > GetLastReferredPlayerCount();
        }

        public void SetNewReferredPlayersChecked()
        {
            SetLastReferredPlayerCount(initializeData.referredPlayers.Length);
        }

        
        private void Unlock()
        {
            ServiceLocator.Find<EventManager>().Propagate(new ReferralCenterUnlockedEvent(), this);
            IsUnlocked = true;
            TrySaveFirstUnlockTime();
        }
        

        private IEnumerator UpdateReferralCenter(WaitForSeconds intervalRefresh)
        {
            while (autoUpdate)
            {
                UpdateReferralCenter(() =>
                {
                    if(HasUnclaimedReward()) OnHasUnclaimedReward?.Invoke();
                }, 
                    failureReason => { });
                
                yield return intervalRefresh;
            }
        }


        private void OverridePrizesData(GoalPrizeData[] goalPrizes)
        {
            foreach (var goalPrize in goalPrizes)
            {
                foreach (var scriptablePrize in ScriptablePrizes)
                {
                    if (goalPrize.goalId == scriptablePrize.goalId && goalPrize.reward!=null && goalPrize.reward.rewards.Length>0)
                        scriptablePrize.UpdateRewardsCount(goalPrize.reward);
                }
            }
        }


        private void SetLastReferredPlayerCount(int count)
        {
            ServiceLocator.Find<UserProfileManager>().SaveData(LAST_REFERRED_PLAYER_COUNT_KEY, count);
        }

        private int GetLastReferredPlayerCount()
        {
            return ServiceLocator.Find<UserProfileManager>().LoadData(LAST_REFERRED_PLAYER_COUNT_KEY, 0);
        }

        private void TrySaveFirstUnlockTime()
        {
            var unlockedTime = PlayerPrefs.GetString("ReferralCenterUnlockedTime", "0");
            if(unlockedTime.Equals("0"))
                PlayerPrefs.SetString("ReferralCenterUnlockedTime", DateTime.UtcNow.ToFileTimeUtc().ToString());
        }

        public DateTime UnlockedTime()
        {
            var longFileTime = long.Parse(PlayerPrefs.GetString("ReferralCenterUnlockedTime", "0"));
            if(longFileTime == 0) return DateTime.MaxValue;
            return DateTime.FromFileTimeUtc(longFileTime);
        }
        
    }

}