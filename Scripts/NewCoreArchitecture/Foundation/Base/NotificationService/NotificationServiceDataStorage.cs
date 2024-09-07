using System;
using Match3.Utility;
using UnityEngine;


namespace Match3.Foundation.Base.NotificationService
{

    public class NotificationServiceDataStorage : INotificationDataStorage
    {

        public void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public void SaveBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public void SaveDateTime(string key, DateTime time)
        {
            PlayerPrefsEx.SetDateTime(key, time);
        }

        public int GetInt(string key, int def)
        {
            return PlayerPrefs.GetInt(key, def);
        }

        public bool GetBool(string key, bool def)
        {
            return PlayerPrefs.GetInt(key, def ? 1 : 0) == 1;
        }

        public DateTime GetDateTime(string key, DateTime def)
        {
            return PlayerPrefsEx.GetDateTime(key, def);
        }
    }
    
}