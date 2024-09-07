using System;
using Match3.Foundation.Base.EventManagement;
using UnityEngine;

namespace Match3.Foundation.Base.NotificationService
{

    [CreateAssetMenu(menuName = "NotificationSystem/BackToGameNotification")]
    public class BackToGameNotification : Notification
    {
        public override bool CheckEvent(GameEvent ev)
        {
            return ev is GameOpenEvent;
        }

        public override bool IsConditionsResolved(INotificationDataStorage dataStorage)
        {
            return true;
        }

        public override TimeSpan GetScheduleTime()
        {
            return TimeSpan.FromSeconds(259200);
        }
    }

}