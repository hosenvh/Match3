using UnityEngine;


namespace Match3.Data.Unity.PersistentTypes
{
    public class PersistentString : PersistantType<string>
    {
        public PersistentString(string key) : base(key)
        {
        }

        public override void Set(string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public override string Get(string defaultValue = default)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
    }
}