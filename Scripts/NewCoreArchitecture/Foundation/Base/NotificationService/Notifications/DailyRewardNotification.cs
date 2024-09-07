using System;
using System.Collections;
using System.Collections.Generic;
using Match3.DailyReward.Game.Handler;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


namespace Match3.Foundation.Base.NotificationService
{
    [CreateAssetMenu(menuName = "NotificationSystem/DailyRewardNotification")]
    public class DailyRewardNotification : Notification
    {
        public override bool CheckEvent(GameEvent ev)
        {
            return ev is DailyRewardNotificationHandler.NotificationEvent;
        }

        public override bool IsConditionsResolved(INotificationDataStorage dataStorage)
        {
            return true;
        }

        public override TimeSpan GetScheduleTime()
        {
            var dailyRewardConfig = ServiceLocator.Find<ServerConfigManager>().data.config.dailyRewardServerConfigData;
            var currentDate = ServiceLocator.Find<ITimeManager>().GetCurrentLocalTimeTime();
            DateTime tomorrowDate = currentDate.AddDays(1);
            DateTime targetDate = new DateTime(tomorrowDate.Year, tomorrowDate.Month, tomorrowDate.Day, dailyRewardConfig.NotificationHour, dailyRewardConfig.NotificationMinute, 0);
            return targetDate - currentDate;
        }
    }
}