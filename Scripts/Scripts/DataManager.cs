using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


namespace Match3
{
    // TODO: Refactor this asshole to be a part of our dataManager, or at least fucking rename it :)
    public class DataManager : IDataManager
    {
        public long GetTimeFromLastSave(string key)
        {
            var time = long.Parse(PlayerPrefs.GetString(key , "-1"));
            if (time < 0)
                return 0;

            var timeManager = ServiceLocator.Find<ITimeManager>();
            return timeManager.GetCurrentServerUnixTime() - time;
        }

        public void SaveServerTimeForKey(string key)
        {
            var timeManager = ServiceLocator.Find<ITimeManager>();
            PlayerPrefs.SetString(key, timeManager.GetCurrentServerUnixTime().ToString());
        }


        public long GetSavedTimeForKey(string key)
        {
            var time = long.Parse(PlayerPrefs.GetString(key , "-1"));
            if (time < 0) time = 0;
            return time;
        }
        
        
        public void SetSavedTimeForKey(string key, string time)
        {
            PlayerPrefs.SetString(key, time);
        }
        
        
        
    }
}