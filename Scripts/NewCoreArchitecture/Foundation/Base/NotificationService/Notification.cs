using System;
using Match3.Foundation.Base.EventManagement;


namespace Match3.Foundation.Base.NotificationService
{
    
    public abstract class Notification : NotificationScriptableData
    {
        public abstract bool CheckEvent(GameEvent ev);
        
        public abstract bool IsConditionsResolved(INotificationDataStorage dataStorage);

        public abstract TimeSpan GetScheduleTime();
    }

}
