using System;


namespace Match3.Foundation.Unity
{


    [Serializable]
    public class ResourceDayConfigAsset : ResourceAsset<DayConfig> { }


#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(ResourceDayConfigAsset))]
    public class ResourceDayConfigConfigEditor : ResourceAssetDrawer<DayConfig> { }

#endif
}
