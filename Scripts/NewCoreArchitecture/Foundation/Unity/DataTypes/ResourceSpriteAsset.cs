using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Match3.Foundation.Unity
{
    [System.Serializable]
    public class ResourceSpriteAsset : ResourceAsset<Sprite> { }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ResourceSpriteAsset))]
    public class ResourceSpriteAssetEditor : ResourceAssetDrawer<Sprite> { }
#endif
}
