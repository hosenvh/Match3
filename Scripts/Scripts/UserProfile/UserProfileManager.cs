using System;
using UnityEngine;

namespace Match3
{
    public class UserProfileManager : IUserProfile
    {
        private const string serverTimeKey = "ServerTimeKey";
        private const string luckySpinnerTimeKey = "LuckySpinnerTimeKey";

        public string GlobalUserId
        {
            get => PlayerPrefs.GetString("globalUserId", "0");
            private set => PlayerPrefs.SetString("globalUserId", value);
        }

        public bool ReferredPlayer
        {
            get => PlayerPrefs.GetInt("ReferredPlayer", 0) == 1;
            set => PlayerPrefs.SetInt("ReferredPlayer", value ? 1 : 0);
        }
        
        public UserProfileManager()
        {
        }
        
        public void Init(Action onCompleted)
        {
            if (IsUserIdEverSet())
                onCompleted();
            else
                AssignNewUserId();

            bool IsUserIdEverSet() => GlobalUserId.Equals("0") == false;

            void AssignNewUserId()
            {
                FetchUserIdBasedOnDevice(
                    onComplete: (userId) =>
                    {
                        GlobalUserId = userId;
                        onCompleted.Invoke();
                    });
            }
        }

        public void FetchUserIdBasedOnDevice(Action<string> onComplete)
        {
            var isSupported = Application.RequestAdvertisingIdentifierAsync((adId, trackingEnabled, error) =>
            {
                onComplete.Invoke(adId);
            });

            if (isSupported == false)
                onComplete.Invoke($"DEVICE_ID_{SystemInfo.deviceUniqueIdentifier}");
        }

        private long GetLuckySpinnerLastUsedTime()
        {
            var value = PlayerPrefs.GetString(luckySpinnerTimeKey, "-1");
            return long.Parse(value);
        }

        public long GetLastLoginTime()
        {
            var value = PlayerPrefs.GetString(serverTimeKey, "-1");
            return long.Parse(value);
        }

        public bool IsLuckySpinnerEnable(int luckySpinnerTime = 24)
        {
            var luckySpinnerLastUsedTime = GetLuckySpinnerLastUsedTime();
            var lastLoginTime = GetLastLoginTime();

            return ((luckySpinnerLastUsedTime <= 0 && lastLoginTime > 0) || ((lastLoginTime - luckySpinnerLastUsedTime) > (luckySpinnerTime * 3600)));
        }

        public void LuckySpinnerUsed()
        {
            PlayerPrefs.SetString(luckySpinnerTimeKey, GetLastLoginTime().ToString());
        }

        public T LoadData<T>(string key, T defaultValue) 
        {
            if (typeof(T) == typeof(int))
            {
                var value = PlayerPrefs.GetInt(key, (int)(object)defaultValue);
                return (T)(object)value;
            } else if (typeof(T) == typeof(float))
            {
                var value = PlayerPrefs.GetFloat(key, (float)(object)defaultValue);
                return (T)(object)value;
            } else if (typeof(T) == typeof(string))
            {
                var value = PlayerPrefs.GetString(key, (string)(object)defaultValue);
                return (T)(object)value;
            }
            return defaultValue;
        }

        public void SaveData<T>(string key, T value)
        {
            if (typeof(T) == typeof(int))
            {
                PlayerPrefs.SetInt(key, (int)(object)value);

            }
            else if (typeof(T) == typeof(float))
            {
                PlayerPrefs.SetFloat(key, (float)(object)value);
            }
            else
            {
                PlayerPrefs.SetString(key, value.ToString());
            }
        }


        public void SetGlobalUserID(string id)
        {
            GlobalUserId = id;
        }
        
    }
}