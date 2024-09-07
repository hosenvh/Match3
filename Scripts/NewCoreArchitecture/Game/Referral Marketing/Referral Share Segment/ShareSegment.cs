using System;
using Match3.Foundation.Base.EventManagement;
using UnityEngine;


namespace Match3.Game.ReferralMarketing.Segments
{
    [Serializable]
    public abstract class ShareSegment : ShareSegmentScriptableData
    {
        private ShareSegmentDataStorage shareSegmentDataStorage;
        private ReferralCenter referralCenter;

        public void Initialize(ShareSegmentDataStorage storage, ReferralCenter referralCenter)
        {
            this.referralCenter = referralCenter;
            shareSegmentDataStorage = storage;
        }

        public void Share(Action<Reward> onShareSucceed, Action onShareFailed)
        {
            if (IsAvailable())
            {
                var segmentReward = reward.GetReward();
                Base.gameManager.OpenPopup<Popup_ShareMenu>().Setup(GetTaggedReferralCode(referralCenter.ReferralCode),
                    (result, appName) =>
                    {
                        if (result == NativeShare.ShareResult.Shared 
                            && Application.internetReachability != NetworkReachability.NotReachable)
                        {
                            Consume();
                            SaveLastShareDate();
                            ApplyReward();
                            ResetInternalState();
                            onShareSucceed(segmentReward);
                            return;
                        }

                        onShareFailed();
                    }).SetCallerSegment(this).SetActiveHintBox(!(segmentReward is EmptyReward));
            }
            else
            {
                Debug.LogError("ShareSegment isn't available but still trying to share");
            }
        }

        public bool IsAvailable()
        {
            if(IsPossibleToRechargeSegment()) 
                RechargeSegment();

            return isEnable && IsReferralCenterAvailable() && !IsReferralCenterProgressCompleted()
                              && IsInternalConditionsSatisfied()
                              && HaveDailyQuota();
        }
        
        
        protected abstract bool IsInternalConditionsSatisfied();
        protected abstract void ResetInternalState();
        public abstract void UpdateInternalState(GameEvent gameEvent);

        
        private string GetTaggedReferralCode(string referralCode)
        {
            return referralCode + tag;
        }
        
        private void ApplyReward()
        {
            reward.GetReward().Apply();
        }

        private bool IsReferralCenterProgressCompleted()
        {
            return referralCenter.Progress() >= 1;
        }
        
        private bool IsReferralCenterAvailable()
        {
            return referralCenter.IsAvailable();
        }

        public bool HaveDailyQuota()
        {
            return RemainingDailyQuota() > 0 || !isLimited;
        }
        
        private int RemainingDailyQuota()
        {
            return shareSegmentDataStorage.GetSegmentDailyQuota(this);
        }

        private void Consume()
        {
            var remainingDailyQuota = RemainingDailyQuota();
            SetRemainingDailyQuota(--remainingDailyQuota);
        }
        
        private DateTime LastShareDate()
        {
            return shareSegmentDataStorage.GetLastTimeSegmentShared(this);
        }

        private void SetRemainingDailyQuota(int count)
        {
            shareSegmentDataStorage.SetSegmentDailyQuota(this, count);
        }

        private void SaveLastShareDate()
        {
            shareSegmentDataStorage.SetSegmentLastTimeShared(this, DateTime.Today);
        }
        

        private bool IsPossibleToRechargeSegment()
        {
            return RemainingDailyQuota() < dailyQuota && DateTime.Today.Date > LastShareDate().Date;
        }

        private void RechargeSegment()
        {
            SetRemainingDailyQuota(dailyQuota);
        }
    }
}