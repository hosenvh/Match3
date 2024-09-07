using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Match3.Foundation.Unity
{
    [System.Serializable]
    public class ResourceButtonAsset : ResourceAsset<Button>
    {
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ResourceButtonAsset))]
    public class ResourceButtonAssetEditor : ResourceAssetDrawer<Button>
    {
    }
    #endif
}