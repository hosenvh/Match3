
using UnityEngine;
using UnityEngine.UI;

public class WasteItemData : IItemData
{
    #region constructors
    public WasteItemData()
    {
    }
    #endregion

    #region public methods
    public ItemType GetItemType() { return global::ItemType.WasteItem; }
    #endregion
}