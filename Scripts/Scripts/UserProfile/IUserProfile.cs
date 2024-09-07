using Match3.Foundation.Base.ServiceLocating;
using System;

namespace Match3
{
    public interface IUserProfile : Service
    {
        string GlobalUserId { get; }

        void Init(Action onCompleted);

        long GetLastLoginTime();
        void LuckySpinnerUsed();

        //luckySpinnerTime in hour
        bool IsLuckySpinnerEnable(int luckySpinnerTime = 24);

        T LoadData<T>(string key, T defaultValue);
        void SaveData<T>(string key, T value);

    }
}