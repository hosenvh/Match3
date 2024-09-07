using UnityEngine;


namespace Match3.Foundation.Unity
{
    [System.Serializable]
    public class ResourceAsset
    {
        public string resourcePath;
    }

    [System.Serializable]
    public class ResourceAsset<T> : ResourceAsset where T : Object
    {
        public T Load()
        {
            return Resources.Load<T>(resourcePath);
        }
    }
}
