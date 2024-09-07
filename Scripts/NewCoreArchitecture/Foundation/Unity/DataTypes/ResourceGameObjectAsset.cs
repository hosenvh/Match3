using UnityEditor;
using UnityEngine;


namespace Match3.Foundation.Unity
{
    [System.Serializable]
    public class ResourceGameObjectAsset : ResourceAsset<GameObject> { }
    
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ResourceGameObjectAsset))]
    public class ResourceGameObjectAssetEditor : ResourceAssetDrawer<GameObject> { }
#endif
    
}