
using UnityEngine;
using UnityEngine.UI;

public class MugItemData : IItemData
{
    #region constructors
    public MugItemData()
    {
    }
    #endregion

    #region public methods
    public ItemType GetItemType() { return global::ItemType.MugItem; }
    #endregion
}