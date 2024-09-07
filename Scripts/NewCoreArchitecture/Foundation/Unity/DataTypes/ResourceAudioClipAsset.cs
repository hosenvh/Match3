using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Match3.Foundation.Unity
{

    [System.Serializable]
    public class ResourceAudioClipAsset : ResourceAsset<AudioClip> { }

    
#if UNITY_EDITOR
    
    [CustomPropertyDrawer(typeof(ResourceAudioClipAsset))]
    public class ResourceAudioClipAssetEditor : ResourceAssetDrawer<AudioClip> { }
    
#endif
    
}