using System;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.ReferralMarketing;
using Match3.Game.ReferralMarketing.Share;
using Match3.Presentation.ReferralMarketing;
using Match3.Utility;
using UnityEngine;


namespace Match3.Game.TaskManagement
{
    public class MainMenuReferralCenterReminderTask : MainMenuTask
    {
        private int firstTimeRemindingDay;
        private int otherTimeRemindingDay;

        private int maxRemindingCount;
        
        private ReferralCenterMainMenuController referralCenterMainMenuController;
        
        
        private bool IsFirstReminder
        {
            get => PlayerPrefsEx.GetBoolean("ReferralCenterReminderFirstTime", true);
            set => PlayerPrefsEx.SetBoolean("ReferralCenterReminderFirstTime", value);
        }

        public int RemindingCount
        {
            get => PlayerPrefs.GetInt("ReferralCenterRemindingCount", 0);
            set => PlayerPrefs.SetInt("ReferralCenterRemindingCount", value);
        }

        public MainMenuReferralCenterReminderTask(ReferralCenterMainMenuController referralCenterMainMenuController)
        {
            this.referralCenterMainMenuController = referralCenterMainMenuController;
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
        }
        
        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            onAbort();
            SetLastRemindingTime();
            RemindingCount++;
            IsFirstReminder = false;
            Base.gameManager.OpenPopup<Popup_ReferralCenterReminder>().Setup(referralCenterMainMenuController);
        }

        protected override bool IsConditionSatisfied()
        {
            if (RemindingCount >= maxRemindingCount) return false;
            
            var referralCenter = ServiceLocator.Find<ReferralCenter>();
            if (!referralCenter.IsAvailable() || referralCenter.Progress() >= 1) return false;
            
            var daysHasToPassed = IsFirstReminder ? firstTimeRemindingDay : otherTimeRemindingDay;

            var isEnoughDaysPassedFromLastShare = !SocialSharing.DoesEverSharingHappened() ||
                                                  (DateTime.UtcNow - SocialSharing.GetLastShareTime()).Days >=
                                                  daysHasToPassed;

            var isEnoughDaysPassedFromLastReminding = IsFirstReminder ||
                                                      (DateTime.UtcNow - LastRemindingTime()).Days >= daysHasToPassed;

            var isEnoughDaysPassedFromReferralUnlocked =
                (DateTime.UtcNow - referralCenter.UnlockedTime()).Days >= daysHasToPassed;
            
            return isEnoughDaysPassedFromReferralUnlocked  && isEnoughDaysPassedFromLastShare && isEnoughDaysPassedFromLastReminding;
        }

        public void SetRemindingDays(int firstTimeDay, int otherTimeDay)
        {
            firstTimeRemindingDay = firstTimeDay;
            otherTimeRemindingDay = otherTimeDay;
        }

        public void SetMaxRemindingCount(int remindingCount)
        {
            maxRemindingCount = remindingCount;
        }

        private void SetLastRemindingTime()
        {
            PlayerPrefs.SetString("LastReferralCenterRemindingTime", DateTime.UtcNow.ToFileTimeUtc().ToString());
        }

        public DateTime LastRemindingTime()
        {
            var fileTime = long.Parse(PlayerPrefs.GetString("LastReferralCenterRemindingTime", "0"));
            if(fileTime == 0) return DateTime.MaxValue;
            var lastDateTime = DateTime.FromFileTimeUtc(fileTime);
            return lastDateTime;
        }
        
    }
}


