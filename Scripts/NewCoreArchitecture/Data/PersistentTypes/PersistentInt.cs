using UnityEngine;


namespace Match3.Data.Unity.PersistentTypes
{
    public class PersistentInt : PersistantType<int>
    {
        public PersistentInt(string key) : base(key)
        {
        }

        public override void Set(int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public override int Get(int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

		public static int operator +(PersistentInt a, PersistentInt b) => a.Get(0) + b.Get(0);
    }
}
        
