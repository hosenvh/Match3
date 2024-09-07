using Match3.Foundation.Base.ServiceLocating;
using Match3.Utility;
using UnityEngine;

namespace Match3.Game
{
    
    public class MapItemPurchaseManager : Service
    {
        
        public bool TryPurchaseItem(MapItem_UserSelect_Single mapItem, int index)
        {
            var profiler = Base.gameManager.profiler;
            var result = profiler.TryConsumeKey(mapItem.costs[index]);
            var currentMapId = Base.gameManager.mapManager.CurrentMap.MapId;
            if (result)
                profiler.SavePurchasableCondition(currentMapId, mapItem.GetItemId(), index, true);
            return result;
        }

        public void SetItemPurchased(string mapId, int userSelectMapItemId , int itemIndex)
        {
            Base.gameManager.profiler.SavePurchasableCondition(mapId, userSelectMapItemId, itemIndex, true);
        }
        
        public bool IsItemPurchasable(MapItem_UserSelect_Single mapItem, int index)
        {
            return mapItem.costs.Length > index && mapItem.costs[index] > 0;
        }

        public bool IsItemAlreadyPurchased(MapItem_UserSelect_Single userSelectItem, int itemIndex)
        {
            var currentMapId = Base.gameManager.mapManager.CurrentMap.MapId;
            return Base.gameManager.profiler.IsPurchasableItemBought(currentMapId, userSelectItem.GetItemId(), itemIndex);
        }
        
        public bool IsItemAlreadyPurchased(string mapId, int userSelectItemId, int itemIndex)
        {
            return Base.gameManager.profiler.IsPurchasableItemBought(mapId, userSelectItemId, itemIndex);
        }
        
        public bool[] GetItemsPurchaseCondition(MapItem_UserSelect_Single mapItem)
        {
            var purchaseConditions = new bool [mapItem.TotalItems()];
            for (var i = purchaseConditions.Length-1; i >=0; --i)
            {
                purchaseConditions[i] = !IsItemPurchasable(mapItem, i) || IsItemAlreadyPurchased(mapItem, i);
            }

            return purchaseConditions;
        }

        
        public MapItemPurchaseManager()
        {
            if (!IsOldSaveDataConverted)
            {
                ConvertAndDeleteSingleMapSaveDataToMultiMap();
                IsOldSaveDataConverted = true;
            }
        }

        private bool IsOldSaveDataConverted
        {
            get => PlayerPrefsEx.GetBoolean("PurchasableItemsDataConversion");
            set => PlayerPrefsEx.SetBoolean("PurchasableItemsDataConversion", value);
        }
        
        private void ConvertAndDeleteSingleMapSaveDataToMultiMap()
        {
            var mapsItemsData = Base.gameManager.mapItemManager.MapsItemsDatabase.MapsItemsData;

            foreach (var mapItemsData in mapsItemsData)
            {
                var mapId = mapItemsData.mapId;
                foreach (var userSelectItem in mapItemsData.userSelectItems)
                {
                    if (!userSelectItem.hasPurchasable) continue;
                    
                    for (int userSelectItemIndexCounter = 0; userSelectItemIndexCounter < userSelectItem.elementsCount; ++userSelectItemIndexCounter)
                    {
                        var purchasableItemConditionDataKey =
                            OldPurchasableItemConditionKey(userSelectItem.itemId, userSelectItemIndexCounter);
                        if (PlayerPrefs.HasKey(purchasableItemConditionDataKey))
                        {
                            var purchaseCondition = PlayerPrefs.GetInt(purchasableItemConditionDataKey, 0) == 1;
                            Base.gameManager.profiler.SavePurchasableCondition(mapId, userSelectItem.itemId, userSelectItemIndexCounter, purchaseCondition);
                            PlayerPrefs.DeleteKey(purchasableItemConditionDataKey);
                        }
                    }
                }
            }
        }

        private string OldPurchasableItemConditionKey(int userSelectId, int itemIndex)
        {
            return $"PurchasableItem_{userSelectId.ToString()}_{itemIndex}";
        }
        
    }
    
}


