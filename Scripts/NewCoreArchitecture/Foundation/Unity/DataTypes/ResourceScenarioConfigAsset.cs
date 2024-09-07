using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



namespace Match3.Foundation.Unity
{

    [System.Serializable]
    public class ResourceScenarioConfigAsset : ResourceAsset<ScenarioConfig> { }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ResourceScenarioConfigAsset))]
    public class ResourceScenarioConfigAssetEditor : ResourceAssetDrawer<ScenarioConfig> { }
    
#endif
    
}