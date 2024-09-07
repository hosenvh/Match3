using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowItemData : IItemData
{
    #region constructors
    public RainbowItemData()
    {
    }
    #endregion

    #region public methods
    public ItemType GetItemType() { return global::ItemType.RainbowItem; }
    #endregion
}