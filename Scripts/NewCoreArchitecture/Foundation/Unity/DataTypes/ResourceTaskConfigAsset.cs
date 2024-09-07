using UnityEditor;


namespace Match3.Foundation.Unity
{
    
    [System.Serializable]
    public class ResourceTaskConfigAsset : ResourceAsset<TaskConfig>
    {
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ResourceTaskConfigAsset))]
    public class ResourceTaskConfigAssetEditor : ResourceAssetDrawer<TaskConfig>
    {
    }

#endif
    
}