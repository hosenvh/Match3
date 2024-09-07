using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.LuckySpinner.TimeBased.Game;
using UnityEngine;


namespace Match3.Foundation.Base.NotificationService
{

    [CreateAssetMenu(menuName = "NotificationSystem/LuckySpinnerReadyNotification")]
    public class LuckySpinnerReadyNotification : Notification
    {
        public override bool CheckEvent(GameEvent ev)
        {
            return ev is TimeBasedLuckySpinnerHandler.TimeBasedLuckSpinnerRewardClaimedEvent;
        }

        public override bool IsConditionsResolved(INotificationDataStorage dataStorage)
        {
            return true;
        }

        public override TimeSpan GetScheduleTime()
        {
            return TimeSpan.FromSeconds(ServiceLocator.Find<ServerConfigManager>().data.config.timeBasedLuckySpinnerServerConfig.WaitingDuration.TotalSeconds);
        }
    }

}