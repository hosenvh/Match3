using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Match3.Foundation.Unity
{
    [Serializable]
    public class ResourceTextureAsset : ResourceAsset<Texture>
    {
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ResourceTextureAsset))]
    public class ResourceTextureAssetEditor : ResourceAssetDrawer<Texture>
    {
    }
    #endif
}