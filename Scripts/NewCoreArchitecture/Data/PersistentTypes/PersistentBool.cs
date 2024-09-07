using Match3.Utility;


namespace Match3.Data.Unity.PersistentTypes
{
    public class PersistentBool : PersistantType<bool>
    {
        public PersistentBool(string key) : base(key)
        {
        }

        public override void Set(bool value)
        {
            PlayerPrefsEx.SetBoolean(key, value);
        }

        public override bool Get(bool defaultValue = false)
        {
            return PlayerPrefsEx.GetBoolean(key, defaultValue);
        }
    }
}