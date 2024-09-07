using System;
using System.Collections.Generic;
using LocalPush;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;



namespace Match3.Foundation.Base.NotificationService
{

    public class NotificationService : Service, EventListener
    {

        private const string NOTIFICATION_LAST_ID_PREFIX = "Notification_LastId_";
        private const int DEFAULT_LAST_NOTIFICATION_ID = -1;

        private const string NOTIFICATION_SCHEDULE_DATE_TIME_PREFIX = "Notification_ScheduleDateTime_";
        
        private readonly NotificationActivationController notificationActivationController;
        private readonly INotificationDataStorage dataStorage;
        private Notification[] notifications;

        private bool hasConfig = false;
        
        
        public  NotificationService(INotificationDataStorage dataStorage)
        {
            this.dataStorage = dataStorage;
            notificationActivationController = new NotificationActivationController(dataStorage);
            ServiceLocator.Find<EventManager>().Register(this);            
        }

        public void Config(Notification[] notifications)
        {
            this.notifications = notifications;
            hasConfig = true;
        }

        
        
        public void ReScheduleWaitingNotifications()
        {
            var waitingNotifications = GetAllWaitingNotifications();
            foreach (var notification in waitingNotifications)
            {
                CancelLastNotification(notification);
                SendAndSaveNotification(notification);
            }
        }
        
        
        
        public bool CheckActive(Notification notif)
        {
            return notificationActivationController.IsNotificationActive(notif);
        }
        
        public void SetActive(Notification notif, bool active)
        {
            notificationActivationController.SaveNotificationActiveness(notif, active);
            if(!active) CancelLastNotification(notif);
        }

        
        
        public void OnEvent(GameEvent evt, object sender)
        {
            if (!hasConfig)
            {
                Debug.LogError("Notification Service Didn't Config");
                return;
            }

            for (var i = notifications.Length - 1; i >= 0; --i)
            {
                var notif = notifications[i];
                if (!notif.CheckEvent(evt) || !notif.IsConditionsResolved(dataStorage)) continue;
                
                if (notificationActivationController.NeedToVerifyActivation(notif))
                {
                    // This is for preventing of Access Closure
                    var notif1 = notif;
                    notificationActivationController.VerifyToActiveFor(notif, active =>
                    {
                        notificationActivationController.SaveNotificationActiveness(notif1, active);
                        
                        if (!active) return;
                        SendAndSaveNotification(notif1);
                    });
                }
                else if (notificationActivationController.IsNotificationActive(notif))
                {
                    CancelLastNotification(notif);
                    SendAndSaveNotification(notif);
                }
            }
        }

        private void CancelLastNotification(Notification notification)
        {
            var lastId = GetNotificationLastId(notification);
            if(lastId != DEFAULT_LAST_NOTIFICATION_ID)
                NotificationManager.Cancel(lastId);
        }

        private void SendAndSaveNotification(Notification notif)
        {
            var id = SendNotification(notif);
            SaveScheduledNotificationData(notif, id);
        }
        
        private int SendNotification(Notification notification)
        {
            Debug.Log($"Notification | Scheduling , {notification.GetType()} in {notification.GetScheduleTime().TotalSeconds} seconds");
            return NotificationManager.Send(
                notification.GetScheduleTime(),
                notification.localizedTitle.ToString(),
                notification.localizedMessage.ToString(),
                notification.notifColor, 
                notification.smallIcon);
        }
        
        private void SaveScheduledNotificationData(Notification notification, int id)
        {
            dataStorage.SaveInt(NOTIFICATION_LAST_ID_PREFIX + notification.id, id);
            
            var delay = notification.GetScheduleTime();
            var notificationDateTime = DateTime.UtcNow + delay;
            dataStorage.SaveDateTime(NOTIFICATION_SCHEDULE_DATE_TIME_PREFIX + notification.id, notificationDateTime);
        }
        

        
        private List<Notification> GetAllWaitingNotifications()
        {
            List<Notification> waitingNotifications = new List<Notification>();
            foreach (var notification in notifications)
            {
                if (GetNotificationLastId(notification) != DEFAULT_LAST_NOTIFICATION_ID)
                {
                    var scheduleTime = GetNotificationScheduleTime(notification);
                    if (scheduleTime > DateTime.UtcNow)
                        waitingNotifications.Add(notification);
                }
            }

            return waitingNotifications;
        }
        
        private int GetNotificationLastId(Notification notification)
        {
            return dataStorage.GetInt(NOTIFICATION_LAST_ID_PREFIX + notification.id, DEFAULT_LAST_NOTIFICATION_ID);
        }

        private DateTime GetNotificationScheduleTime(Notification notification)
        {
            return dataStorage.GetDateTime(NOTIFICATION_SCHEDULE_DATE_TIME_PREFIX + notification.id, DateTime.UtcNow);
        }

        
    }

}