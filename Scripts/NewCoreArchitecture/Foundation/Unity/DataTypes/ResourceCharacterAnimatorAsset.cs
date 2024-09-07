using Spine.Unity;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Match3.Foundation.Unity
{
    [System.Serializable]
    public class ResourceCharacterAnimatorAsset : ResourceAsset<CharacterAnimator> { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ResourceCharacterAnimatorAsset))]
    public class ResourceCharacterAnimatorAssetEditor : ResourceAssetDrawer<CharacterAnimator> { }
#endif
}
