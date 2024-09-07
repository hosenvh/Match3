using System;
using Match3.Foundation.Base.EventManagement;
using UnityEngine;


namespace Match3.Foundation.Base.NotificationService
{

    [CreateAssetMenu(menuName = "NotificationSystem/FullLifeNotification")]
    public class FullLifeNotification : Notification
    {

        [Tooltip("Minimum life count that should be consumed in order to send notification")]
        public int minimumConsumedLifeCount;
        
        public override bool CheckEvent(GameEvent ev)
        {
            return ev is LifeChangeEvent;
        }

        public override bool IsConditionsResolved(INotificationDataStorage dataStorage)
        {
            return (global::Base.gameManager.profiler.LifeCount <=
                    global::Base.gameManager.profiler.GetMaxLifeCount() - minimumConsumedLifeCount);
        }

        public override TimeSpan GetScheduleTime()
        {
            var gameManager = global::Base.gameManager;
            return TimeSpan.FromSeconds((long)gameManager.profiler.NextFullLifeTime());
        }
        
    }

}