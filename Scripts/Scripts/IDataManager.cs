using Match3.Foundation.Base.ServiceLocating;
using System;

namespace Match3
{
    // TODO: Refactor or at least rename this
    public interface IDataManager : Service
    {
        long GetTimeFromLastSave(string key);
        void SaveServerTimeForKey(string key);
        void SetSavedTimeForKey(string key, string time);
        long GetSavedTimeForKey(string key);
    }
}