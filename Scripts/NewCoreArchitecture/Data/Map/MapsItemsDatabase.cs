using System.Collections.Generic;
using UnityEngine;



namespace Match3.Data.Map
{

    
    
    [System.Serializable]
    public class MapItemUserSelectMetadata
    {
        public string itemType;
        public int itemId;
        public int elementsCount;
        public bool hasPurchasable;
        
        public MapItemUserSelectMetadata(MapItem_UserSelect userSelectMapItem)
        {
            itemType = userSelectMapItem.GetType().AssemblyQualifiedName;
            itemId = userSelectMapItem.GetItemId();
            elementsCount = userSelectMapItem.TotalNormalItems();
            
            if (userSelectMapItem is MapItem_UserSelect_Single singleUserSelect)
            {
                hasPurchasable = singleUserSelect.costs != null && singleUserSelect.costs.Length > 0;
            }
        }
    }

    
    
    
    [System.Serializable]
    public class MapItemStateMetaData
    {
        public string itemType;
        public int itemId;
        public int initIndex;
        
        public MapItemStateMetaData(MapItem_State stateMapItem)
        {
            itemType = stateMapItem.GetType().AssemblyQualifiedName;
            itemId = stateMapItem.GetItemId();
            if (stateMapItem is MapItem_State_Single singleStateItem)
                initIndex = singleStateItem.initIndex;
        }
    }
    
    
    
    
    [System.Serializable]
    public class MapItemsData
    {
        public string mapId;
        public List<MapItemUserSelectMetadata> userSelectItems = new List<MapItemUserSelectMetadata>();
        public List<MapItemStateMetaData> stateItems = new List<MapItemStateMetaData>();

        public MapItemsData(string mapId)
        {
            this.mapId = mapId;
        }
    }
    
    
    
    
    [CreateAssetMenu(menuName = "Map/MapsItemsDatabase", fileName = "MapsItemsDatabase")]
    public class MapsItemsDatabase : ScriptableObject
    {
        [SerializeField] private List<MapItemsData> mapsItemsData = new List<MapItemsData>();
        public List<MapItemsData> MapsItemsData => mapsItemsData;

        
        public void InsertOrUpdateUserSelect(string mapId, MapItem_UserSelect userSelectItem)
        {
            MapItemsData mapItemsData = TryGetMapItemsData(mapId) ?? CreateMapItemsData(mapId);

            var itemMetadata = new MapItemUserSelectMetadata(userSelectItem);
            InsertOrUpdateUserSelectMetaDataInList(mapItemsData.userSelectItems, itemMetadata);
        }

        public void InsertOrUpdateState(string mapId, MapItem_State stateItem)
        {
            MapItemsData mapItemsData = TryGetMapItemsData(mapId) ?? CreateMapItemsData(mapId);

            var itemMetadata = new MapItemStateMetaData(stateItem);
            InsertOrUpdateStateMetaDataInList(mapItemsData.stateItems, itemMetadata);
        }

        private void InsertOrUpdateUserSelectMetaDataInList(List<MapItemUserSelectMetadata> userSelectsList, MapItemUserSelectMetadata itemUserSelectMetadata)
        {
            var existsUserSelectMetadata = userSelectsList.Find(x => x.itemId == itemUserSelectMetadata.itemId);
            if (existsUserSelectMetadata != null)
            {
                var inListIndex = userSelectsList.IndexOf(existsUserSelectMetadata);
                userSelectsList[inListIndex] = itemUserSelectMetadata;
            }
            else
                userSelectsList.Add(itemUserSelectMetadata);
        }

        private void InsertOrUpdateStateMetaDataInList(List<MapItemStateMetaData> statesList, MapItemStateMetaData itemStateMetadata)
        {
            var existsStateMetaData = statesList.Find(x => x.itemId == itemStateMetadata.itemId);
            if (existsStateMetaData != null)
            {
                var inListIndex = statesList.IndexOf(existsStateMetaData);
                statesList[inListIndex] = itemStateMetadata;
            }
            else
                statesList.Add(itemStateMetadata);
        }
        
        private MapItemsData TryGetMapItemsData(string mapId)
        {
            foreach (var mapItemsDatabase in mapsItemsData)
            {
                if (mapItemsDatabase.mapId.Equals(mapId))
                {
                    return mapItemsDatabase;
                }
            }
            return null;
        }

        private MapItemsData CreateMapItemsData(string mapId)
        {
            var mapItemsData = new MapItemsData(mapId);
            mapsItemsData.Add(mapItemsData);
            return mapItemsData;
        }
        
    }
    
}


