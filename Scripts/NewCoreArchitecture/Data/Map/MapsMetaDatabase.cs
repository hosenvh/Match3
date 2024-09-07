using System.Collections.Generic;
using UnityEngine;



namespace Match3.Data.Map
{
    
    [CreateAssetMenu(menuName = "Map/MapsMetaDatabase", fileName = "MapsMetaDatabase")]
    public class MapsMetaDatabase : ScriptableObject
    {
        [SerializeField] private List<MapMetaData> mapsMetadata = new List<MapMetaData>();
        public List<MapMetaData> MapsMetadata => mapsMetadata;
        
        public void InsertOrUpdateMapMetadata(MapMetaData mapMetaData)
        {
            var existedMapMetadata = mapsMetadata.Find(x => x.mapId == mapMetaData.mapId);
            if (existedMapMetadata != null)
                existedMapMetadata.mapResourcesPath = mapMetaData.mapResourcesPath;
            else
                mapsMetadata.Add(mapMetaData);
        }
    }
    
    
    [System.Serializable]
    public class MapMetaData
    {
        public string mapId;
        public string mapResourcesPath;

        public MapMetaData(string mapId, string mapResourcesPath)
        {
            this.mapId = mapId;
            this.mapResourcesPath = mapResourcesPath;
        }
    }
    
}


