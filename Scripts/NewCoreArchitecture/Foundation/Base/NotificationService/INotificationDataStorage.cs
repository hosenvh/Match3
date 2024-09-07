

using System;

namespace Match3.Foundation.Base.NotificationService
{

    public interface INotificationDataStorage
    {

        void SaveInt(string key, int value);

        void SaveBool(string key, bool value);

        void SaveDateTime(string key, DateTime time);

        int GetInt(string key, int def);

        bool GetBool(string key, bool def);

        DateTime GetDateTime(string key, DateTime def);
    }

}