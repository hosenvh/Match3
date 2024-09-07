using System;
using UnityEngine;


namespace Match3.Data.Unity.PersistentTypes
{

    public class PersistantDateTime : PersistantType<DateTime>
    {
        public PersistantDateTime(string key) : base(key)
        {
        }

        public override void Set(DateTime value)
        {
            PlayerPrefs.SetString(key, value.ToString("G"));
        }

        public override DateTime Get(DateTime defaultValue = default)
        {
            var stringDate = PlayerPrefs.GetString(key, "");
            return stringDate.IsNullOrEmpty() ? defaultValue : DateTime.Parse(stringDate);
        }
    }

}