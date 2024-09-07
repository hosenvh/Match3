using Spine.Unity;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Match3.Foundation.Unity
{
    [System.Serializable]
    public class ResourceSkeletonGraphicAsset : ResourceAsset<SkeletonGraphic> { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ResourceSkeletonGraphicAsset))]
    public class ResourceSkeletonGraphicAssetEditor : ResourceAssetDrawer<SkeletonGraphic> { }
#endif
}
