using Match3.Data.Unity.PersistentTypes;
using SeganX;


namespace Match3.Data.Unity
{
    [System.Serializable]
    public class InfinityItem
    {
    
        private const string InfinityItemKeyPrefix = "InfinityItem_";
        public readonly string ItemId;

        private PersistentLong persistentStartTime;
        private PersistentInt  persistentDurationTime;
    
    
    
        public InfinityItem(string itemId)
        {
            ItemId = itemId;
        
            persistentStartTime    = new PersistentLong(GetStartTimeDataKey());
            persistentDurationTime = new PersistentInt(GetDurationTimeDataKey());
        }
    
    
        public void AddInfinityDuration(int durationInSecond)
        {
            if (IsAvailable())
            {
                var currentDuration = persistentDurationTime.Get();
                persistentDurationTime.Set(currentDuration + durationInSecond);
            }
            else
            {
                persistentStartTime.Set((int)Utilities.NowTimeUnix());
                persistentDurationTime.Set(durationInSecond);
            }
        }
    
        public bool IsAvailable()
        {
            var endTime = persistentStartTime + persistentDurationTime;
            var nowTime = Utilities.NowTimeUnix();
            return IsTimeValid() && HasRemainingTime();

            bool IsTimeValid() => nowTime >= persistentStartTime.Get();
            bool HasRemainingTime() => nowTime < endTime;
        }

        public int RemainingDuration()
        {
            var endTime = persistentStartTime + persistentDurationTime;
            var nowTime = Utilities.NowTimeUnix();
            return (int)(endTime - nowTime);
        }

        private string GetStartTimeDataKey()
        {
            return InfinityItemKeyPrefix + ItemId + "_StartTime";
        }
    
        private string GetDurationTimeDataKey()
        {
            return InfinityItemKeyPrefix + ItemId + "_DurationTime";
        }
    
    }

}

