#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Match3.Foundation.Unity
{
    [System.Serializable]
    public class ResourceMapItemElementAsset : ResourceAsset<MapItem_Element> { }
    
#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ResourceMapItemElementAsset))]
    public class ResourceMapItemElementAssetEditor : ResourceAssetDrawer<MapItem_Element> { }
    
#endif

}