using System.Collections.Generic;
using Match3.Data.Map;
using UnityEngine;



namespace Match3.Foundation.Unity
{

    public class MapSelectorAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        public List<string> maps;
        
        public MapSelectorAttribute()
        {
            maps = AssetEditorUtilities.FindAssetsByType<MapsIDsDatabase>()[0].mapIDs;
        }
#endif    
    }

}


