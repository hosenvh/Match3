namespace Match3.Data.Unity.PersistentTypes
{
    public abstract class PersistantType<T>
    {
        protected readonly string key;

        public PersistantType(string key)
        {
            this.key = key;
        }

        public abstract void Set(T value);
        public abstract T Get(T defaultValue = default);
    }
}