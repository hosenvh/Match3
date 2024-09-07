using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CloudSave;
using UnityEngine;

namespace Match3.CloudSave
{

    public class MapItemDataHandler : ICloudDataHandler
    {

        private const string MapItemUserSelectKey = "mapItemUserSelect";
        private const string MapItemStateKey = "mapItemState";
        
        
        public void CollectData(ICloudDataStorage cloudStorage)
        {
            var mapItemCenter = Base.gameManager.mapItemManager;
            var allMapsItems = mapItemCenter.MapsItemsDatabase.MapsItemsData;

            var stringBuilder = new StringBuilder();
            int itemId;
            
            foreach (var mapItems in allMapsItems)
            {
                var mapId = mapItems.mapId;
                foreach (var userSelect in mapItems.userSelectItems)
                {
                    itemId = userSelect.itemId;

                    if (stringBuilder.Length > 0) stringBuilder.Append(',');
                    stringBuilder.Append(
                        $"{itemId.ToString()}:{mapItemCenter.GetUserSelectItemSelectedIndex(userSelect, mapId).ToString()}:{mapId}");
                }
            }
            cloudStorage.SetString(MapItemUserSelectKey, stringBuilder.ToString());
            stringBuilder.Clear();
            
            foreach (var mapItems in allMapsItems)
            {
                var mapId = mapItems.mapId;
                foreach (var userState in mapItems.stateItems)
                {
                    var stateType = Type.GetType(userState.itemType);
                    if (stateType != typeof(MapItem_State_Single))
                        continue;
                    
                    itemId = userState.itemId;
                    if (stringBuilder.Length > 0) stringBuilder.Append(',');
                    stringBuilder.Append($"{itemId.ToString()}:{mapItemCenter.GetStateItemStateIndex(userState, mapId).ToString()}:{mapId}");
                }
            }
            cloudStorage.SetString(MapItemStateKey, stringBuilder.ToString());
            stringBuilder.Clear();
        }

        
        public void SpreadData(ICloudDataStorage cloudStorage)
        {
            var mapItemCenter = Base.gameManager.mapItemManager;

            var userSelectStringDatas = cloudStorage.GetString(MapItemUserSelectKey).Split(',');
            var stateStringDatas = cloudStorage.GetString(MapItemStateKey).Split(',');
            
            foreach (var userSelectStringData in userSelectStringDatas)
            {
                var userSelectData = userSelectStringData.Split(':');
                var mapId = GetBackwardSupportMapIdFromRestoredData(userSelectData);
                mapItemCenter.SaveUserSelectItemSelectedIndex(int.Parse(userSelectData[0]), mapId, int.Parse(userSelectData[1]));
            }
            
            foreach (var stateStringData in stateStringDatas)
            {
                var stateData = stateStringData.Split(':');
                var mapId = GetBackwardSupportMapIdFromRestoredData(stateData);
                mapItemCenter.SaveStateItemStateIndex(int.Parse(stateData[0]), mapId, int.Parse(stateData[1]));
            }
        }

        private string GetBackwardSupportMapIdFromRestoredData(string[] itemData)
        {
            if (itemData.Length > 2) return itemData[2];
            return Base.gameManager.mapManager.DefaultMapId;
        }
    }

}