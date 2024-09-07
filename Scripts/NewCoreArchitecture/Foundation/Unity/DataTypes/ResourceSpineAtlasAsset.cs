using System;
using Spine.Unity;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Match3.Foundation.Unity
{
    [Serializable]
    public class ResourceSpineAtlasAsset : ResourceAsset<AtlasAsset>
    {
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ResourceSpineAtlasAsset))]
    public class ResourceSpineAtlasAssetEditor : ResourceAssetDrawer<AtlasAsset>
    {
    }
    #endif
}