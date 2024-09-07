using System;
using System.Globalization;
using UnityEngine;

namespace Match3.Utility
{
    public static class PlayerPrefsEx
    {
        public static void SetBoolean(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public static bool GetBoolean(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue? 1 : 0) == 1;
        }

        public static void SetDateTime(string key, DateTime time)
        {
            PlayerPrefs.SetString(key, time.ToString(CultureInfo.InvariantCulture));
        }

        public static DateTime GetDateTime(string key, DateTime defaultTime)
        {
            var stringTime = PlayerPrefs.GetString(key, "");
            if (stringTime.IsNullOrEmpty())
                return defaultTime;
            else
            {
                return DateTime.Parse(stringTime, CultureInfo.InvariantCulture);
            }
        }
    }
}
