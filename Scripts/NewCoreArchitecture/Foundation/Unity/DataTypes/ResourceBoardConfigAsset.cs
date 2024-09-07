using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Match3.Foundation.Unity
{
    [Serializable]
    public class ResourceBoardConfigAsset : ResourceAsset<BoardConfig> { }

    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ResourceBoardConfigAsset))]
    public class ResourceBoardConfigEditor : ResourceAssetDrawer<BoardConfig> { }

    #endif
}
