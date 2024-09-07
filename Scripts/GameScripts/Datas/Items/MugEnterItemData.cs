
using UnityEngine;
using UnityEngine.UI;

public class MugEnterItemData : IItemData
{
    #region constructors
    public MugEnterItemData()
    {
    }
    #endregion

    #region public methods
    public ItemType GetItemType() { return global::ItemType.MugEnterItem; }
    #endregion
}