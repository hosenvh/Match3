using UnityEngine;


namespace Match3.Data.Unity.PersistentTypes
{
    [System.Serializable]
    public class PersistentLong : PersistantType<long>
    {
        public PersistentLong(string key) : base(key)
        {
        }

        public override void Set(long value)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }

        public override long Get(long defaultValue = 0)
        {
            return long.Parse(PlayerPrefs.GetString(key, defaultValue.ToString()));
        }

        public static long operator +(PersistentInt a, PersistentLong b) => a.Get(0) + b.Get(0);
        public static long operator +(PersistentLong a, PersistentInt b) => a.Get(0) + b.Get(0);
        public static long operator +(PersistentLong a, PersistentLong b) => a.Get(0) + b.Get(0);
    }
}