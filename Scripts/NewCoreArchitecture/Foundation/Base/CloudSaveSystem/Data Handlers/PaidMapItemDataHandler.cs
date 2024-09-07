using System;
using System.Runtime.CompilerServices;
using System.Text;
using CloudSave;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using UnityEngine;


namespace Match3.CloudSave
{
    public class PaidMapItemDataHandler : ICloudDataHandler
{
    private const string PurchasedMapItemKey = "purchasedMapItem";
    
    
    public void CollectData(ICloudDataStorage cloudStorage)
    {
        var mapItemPurchaseManager = ServiceLocator.Find<MapItemPurchaseManager>();
        StringBuilder stringBuilder = new StringBuilder();

        var allMapsItems = Base.gameManager.mapItemManager.MapsItemsDatabase.MapsItemsData;
        foreach (var mapItemsData in allMapsItems)
        {
            var mapId = mapItemsData.mapId;
            
            foreach (var userSelectItem in mapItemsData.userSelectItems)
            {
                if(!userSelectItem.hasPurchasable) continue;
                var itemIndex = userSelectItem.elementsCount;
                for (; itemIndex >= 0; --itemIndex)
                {
                    if (!mapItemPurchaseManager.IsItemAlreadyPurchased(mapId, userSelectItem.itemId, itemIndex)) continue;
                
                    if (stringBuilder.Length > 0) stringBuilder.Append(',');
                    stringBuilder.Append($"{userSelectItem.itemId.ToString()}:{itemIndex}:{mapId}");
                }
            }
        }

        cloudStorage.SetString(PurchasedMapItemKey, stringBuilder.ToString());
    }

    
    public void SpreadData(ICloudDataStorage cloudStorage)
    {
        var mapItemPurchaseManager = ServiceLocator.Find<MapItemPurchaseManager>();
        var purchasedItemsString = cloudStorage.GetString(PurchasedMapItemKey);

        var itemStrings = purchasedItemsString.Split( new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
        foreach (var itemString in itemStrings)
        {
            var purchasedItemData = itemString.Split(':');

            var mapId = GetBackwardSupportMapId(purchasedItemData);
            
            mapItemPurchaseManager.SetItemPurchased(mapId , int.Parse(purchasedItemData[0]),
                int.Parse(purchasedItemData[1]));
        }
    }

    
    private string GetBackwardSupportMapId(string[] purchasedItemData)
    {
        if (purchasedItemData.Length > 2)
            return purchasedItemData[2];
        return Base.gameManager.mapManager.DefaultMapId;
    }
    
}

}

