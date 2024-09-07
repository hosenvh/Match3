using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Match3.Foundation.Unity
{
    [System.Serializable]
    public class ResourceMaterialAsset : ResourceAsset<Material>
    {
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ResourceMaterialAsset))]
    public class ResourceMaterialAssetEditor : ResourceAssetDrawer<Material>
    {
    }
    #endif
}